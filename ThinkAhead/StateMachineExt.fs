namespace Celeste.Mod.ThinkAhead

open Monocle
open System
open System.Collections
open System.Reflection

type StateMachineExt() =
    static member AddState (machine: StateMachine)
                           (update: unit -> int)
                           (coroutine: (unit -> IEnumerator) option)
                           (beginning: (unit -> unit) option)
                           (ending: (unit -> unit) option)
                           : int =
        let mutable updates: Func<int> [] =
            StateMachineExt.StateMachine_updates.GetValue(machine) :?> Func<int> []
        let mutable coroutines: Func<IEnumerator> [] =
            StateMachineExt.StateMachine_coroutines.GetValue(machine) :?> Func<IEnumerator> []
        let mutable begins: Action [] =
            StateMachineExt.StateMachine_begins.GetValue(machine) :?> Action []
        let mutable ends: Action [] =
            StateMachineExt.StateMachine_ends.GetValue(machine) :?> Action []

        let nextIndex = updates.Length
        
        Array.Resize(&updates, updates.Length + 1)
        Array.Resize(&coroutines, coroutines.Length + 1)
        Array.Resize(&begins, begins.Length + 1)
        Array.Resize(&ends, ends.Length + 1)
        
        StateMachineExt.StateMachine_updates.SetValue(machine, updates)
        StateMachineExt.StateMachine_coroutines.SetValue(machine, coroutines)
        StateMachineExt.StateMachine_begins.SetValue(machine, begins)
        StateMachineExt.StateMachine_ends.SetValue(machine, ends)
        
        let csUpdate: Func<int> = Func<_>(update)
        let csCoroutine: Func<IEnumerator> = match coroutine |> Option.map (fun f -> Func<_>(f)) with
                                             | Some x -> x
                                             | None -> null
        let csBeginning: Action = match beginning |> Option.map (fun f -> Action(f)) with
                                  | Some x -> x
                                  | None -> null
        let csEnding: Action = match ending |> Option.map (fun f -> Action(f)) with
                               | Some x -> x
                               | None -> null
        
        machine.SetCallbacks(nextIndex, csUpdate, csCoroutine, csBeginning, csEnding)

        nextIndex

    static member val private StateMachine_updates: FieldInfo =
        typeof<StateMachine>.GetField("updates", BindingFlags.Instance ||| BindingFlags.NonPublic)
    static member val private StateMachine_coroutines: FieldInfo =
        typeof<StateMachine>.GetField("coroutines", BindingFlags.Instance ||| BindingFlags.NonPublic)
    static member val private StateMachine_begins: FieldInfo =
        typeof<StateMachine>.GetField("begins", BindingFlags.Instance ||| BindingFlags.NonPublic)
    static member val private StateMachine_ends: FieldInfo =
        typeof<StateMachine>.GetField("ends", BindingFlags.Instance ||| BindingFlags.NonPublic)
