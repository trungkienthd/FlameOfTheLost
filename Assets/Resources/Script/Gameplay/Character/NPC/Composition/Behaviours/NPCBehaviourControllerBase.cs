using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCBehaviourControllerBase : CharacterBehaviourControllerBase
    {
        protected NPCRootController NPCRootController => this.CharacterRootController as NPCRootController;

        public NPCRootController GetNPC() => this.NPCRootController;
    }
}
