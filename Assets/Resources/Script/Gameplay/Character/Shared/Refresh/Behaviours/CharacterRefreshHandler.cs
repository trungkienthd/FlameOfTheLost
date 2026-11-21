namespace FlameOfTheLost
{
    public class CharacterRefreshHandler : CharacterBehaviourControllerBase
    {
        public virtual void Refresh(CharacterRefreshTarget target,  CharacterRefreshOrigin refreshOrigin)
        {
            CharacterRefreshUtility.RefreshCharacter
            (
                character: this.CharacterRootController,
                position: target.Position,
                rotation: target.Rotation,
                facing: target.Facing,
                refreshOrigin: refreshOrigin
            );
        }
    }
}
