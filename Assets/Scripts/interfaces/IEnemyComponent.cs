public interface IEnemyComponent {

	void Initialize (Player[] players, GameplayMode gameplayMode, PlayArea playArea);

	void LevelReset ();

}
