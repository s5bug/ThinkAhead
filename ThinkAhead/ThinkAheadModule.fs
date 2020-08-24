namespace Celeste.Mod.ThinkAhead

open Celeste
open Celeste.Mod
open Microsoft.Xna.Framework
open MonoMod.Utils

type ThinkAheadModule() =
    inherit EverestModule()
    
    override this.Load() =
        On.Celeste.Player.add_ctor(this.Player_ctorHook)
    override this.Unload() =
        On.Celeste.Player.remove_ctor(this.Player_ctorHook)
    
    member this.Player_ctor (orig: On.Celeste.Player.orig_ctor)
                           (self: Player)
                           (position: Vector2)
                           (spriteMode: PlayerSpriteMode)
                           : unit =
        orig.Invoke(self, position, spriteMode)
        
        // let stAhead = StateMachineExt.AddState self.StateMachine (this.StAheadUpdate)
        
        
    member this.Player_ctorHook = On.Celeste.Player.hook_ctor(this.Player_ctor)
