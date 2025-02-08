namespace Scarlet.Core.Assets.Enums
{
	public enum AssetType
	{
		None = 0,

		// 프리팹 관련
		Prefab = 100,
		Character = 101,
		Environment = 102,
		Effect = 103,
		UI = 104,

		// 비주얼 에셋
		Texture = 200,
		Material = 201,
		Sprite = 202,
		Animation = 203,
		Shader = 204,
		VFX = 205,

		// 오디오
		AudioClip = 300,
		AudioMixer = 301,

		// UI 관련
		UISprite = 400,
		UIFont = 401,
		UIAnimation = 402,

		// 데이터 에셋
		ScriptableObject = 500,
		CharacterData = 501,
		ItemData = 502,
		SkillData = 503,
		QuestData = 504,
		DialogueData = 505,
		LocalizationData = 506,
		BalanceData = 507,

		// 씬 관련
		Scene = 600,
		LightingData = 601,
		NavMesh = 602,

		// 설정 관련
		GameSettings = 700,
		InputSettings = 701,
		AudioSettings = 702,

		// 런타임 데이터
		SaveData = 800,
		PlayerProfile = 801,
		GameState = 802,

		// 기타
		Custom = 900,
		Bundle = 901,

		// Raw
		Raw = 1000, // binary
		Data = 1001, // json
		Text = 1002,
	}

}
