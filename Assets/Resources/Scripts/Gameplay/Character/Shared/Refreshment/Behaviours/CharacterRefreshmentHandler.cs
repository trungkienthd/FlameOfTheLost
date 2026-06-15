namespace FlameOfTheLost
{
    public class CharacterRefreshmentHandler : CharacterBehaviourControllerBase
    {
        public virtual void Refresh(SpawnResolveResult spawnResult,  CharacterPlacementSource placementSource)
        {
            CharacterRefreshmentUtility.RefreshCharacter
            (
                character: this.CharacterRootController,
                position: spawnResult.Position,
                rotation: spawnResult.Rotation,
                facing: spawnResult.Facing,
                placementSource: placementSource
            );
        }
    }
}
