using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FunManager : MonoBehaviour
{
	public static FunManager Instance;
	public List<AchievementScriptableObjects> stateObjects;
	SkinObject skinState;
	AchievementScriptableObjects achievementState;
	[HideInInspector] public UnityAction SceneChanged;

	void Awake()
	{
		if (Instance != null)
		{
			GetComponent<DontDestroyOnLoad>().RemoveCopies();
			return;
		}

		if (SaveManager.Load("getThing", out GetThing data))
		{
			skinState = data.skin == -1 ? null : SkinMagazine.Instance.skins[data.skin];
			achievementState = data.achievement == -1 ? null : Achievements.Instance.achievements[data.achievement];
		}
		else
			SaveManager.Save(
				"getThing",
				new GetThing()
				{
					skin = SkinMagazine.Instance.skins.IndexOf(skinState),
					achievement = Achievements.Instance.achievements.IndexOf(achievementState)
				}
			);

		Instance = this;
	}
	void Start()
	{
		SceneChanged += () => Settings.Instance.GetComponent<Canvas>().worldCamera = Camera.main;
		SceneChanged += () => SkinMagazine.Instance?.LoadSkins();
		SceneChanged += () => Settings.Instance?.LoadSettings();

		SceneChanged?.Invoke();
	}
	public void GetAchievement(AchievementScriptableObjects obj)
	{
		ChangeState(AdvancementType.Skin, obj.skin);
		ChangeState(AdvancementType.Achievement, achieve: obj);

		if (obj.skin != null)
			SkinMagazine.Instance?.GetSkin(obj.skin);
	}
	public void ChangeState(AdvancementType type, SkinObject skin = null, AchievementScriptableObjects achieve = null)
	{
		int skinObjID = skin == null ? -1 : SkinMagazine.Instance.skins.IndexOf(skin),
			achievementsObjID = achieve == null ? -1 : Achievements.Instance.achievements.IndexOf(achieve),

			skinStateID = skinState == null ? -1 : SkinMagazine.Instance.skins.IndexOf(skinState),
			achievementsStateID = achievementState == null ? -1 : Achievements.Instance.achievements.IndexOf(achievementState);

		SaveManager.Save(
			"getThing",
			new GetThing()
			{
				achievement = type == AdvancementType.Skin ? achievementsStateID : achievementsObjID, // не менять если AdvancementType.Skin, иначе поменять
				skin = type == AdvancementType.Skin ? skinObjID : skinStateID // поменять если AdvancementType.Skin, иначе не менять
			}
		);

		if (type == AdvancementType.Skin)
			skinState = skin;
		else
		{
			achievementState = achieve;
			Achievements.Instance?.GetAchievement(achieve);
		}
	}
}

public enum AdvancementType
{
	Achievement,
	Skin
}