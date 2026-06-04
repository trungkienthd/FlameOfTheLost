namespace FlameOfTheLost
{
    public class CharacterPlacementHandler : CharacterBehaviourBaseController
    {
        public virtual void PlaceCharacter(SpawnResolveResult spawnResult,  CharacterPlacementSourceType placementSourceType)
        {
            CharacterPlacementUtility.PlaceAndRefreshCharacter
            (
                character: this.CharacterRootController.gameObject,
                position: spawnResult.Position,
                rotation: spawnResult.Rotation,
                facing: spawnResult.Facing,
                spawnSourceType: placementSourceType
            );
        }
    }
}
