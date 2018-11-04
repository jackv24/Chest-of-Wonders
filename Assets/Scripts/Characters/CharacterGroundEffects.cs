using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control playing ground effects for characters (footsteps, jumping, landing, sounds and particles, etc)
/// </summary>
public class CharacterGroundEffects : MonoBehaviour
{
	public SoundType soundType;

	[System.Serializable]
	public class GroundEffects
	{
		public SoundEventRandom footstepSound;
		public SoundEventSingle jumpSound;
		public SoundEventSingle landSound;

		public TrailParticles trailEffect;
		public GameObject jumpEffect;
		public GameObject landEffect;
	}

	[System.Serializable]
	public class MappedGroundEffect
	{
		public GroundType groundType;
		public GroundEffects groundEffects;
	}

	//Use a public array with a pair class for easy inspector assignment, but swap for a Dictionary at runtime for performance
	[SerializeField] private List<MappedGroundEffect> supportedGroundEffects = new List<MappedGroundEffect>();
	private Dictionary<GroundType, GroundEffects> groundEffects;

	private List<GroundTypeRegion> groundTypeRegions = new List<GroundTypeRegion>();
	private GroundEffects currentGroundEffects;

	private TrailParticles currentTrailEffect;

    private PlayerWallReact playerWallReact;
    private bool didJustWallJump;

	private CharacterMove characterMove;

	private void OnValidate()
	{
		//Make sure we don't have duplicates
		List<GroundType> foundTypes = new List<GroundType>();

		for(int i = 0; i < supportedGroundEffects.Count; i++)
		{
			if(!foundTypes.Contains(supportedGroundEffects[i].groundType))
				foundTypes.Add(supportedGroundEffects[i].groundType);
			else
				Debug.LogWarning("Can't have more than one of the same mapped ground type!", this);
		}
	}

	private void Awake()
	{
		characterMove = GetComponent<CharacterMove>();

        playerWallReact = GetComponent<PlayerWallReact>();
        if (playerWallReact)
            playerWallReact.OnWallJumped += () => didJustWallJump = true;
	}

	private void Start()
	{
		//Transfer ground effects into dictionary for performance (no checking is necessary here since we did that in OnValidate)
		groundEffects = new Dictionary<GroundType, GroundEffects>();
		foreach (MappedGroundEffect mappedGroundEffect in supportedGroundEffects)
		{
			if (mappedGroundEffect.groundType)
				groundEffects.Add(mappedGroundEffect.groundType, mappedGroundEffect.groundEffects);
		}

		if (characterMove)
		{
			characterMove.OnJump += () =>
			{
				if (currentGroundEffects != null)
				{
					currentGroundEffects.jumpSound.Play(transform.position, soundType);
					currentGroundEffects.jumpEffect.SpawnPooled(transform.position);
				}
			};

			characterMove.OnGrounded += () =>
			{
				if (currentGroundEffects != null)
				{
					currentGroundEffects.landSound.Play(transform.position, soundType);
					currentGroundEffects.landEffect.SpawnPooled(transform.position);
				}
			};
		}

		GameManager.instance.OnLevelLoaded += UpdateGroundEffects;
		UpdateGroundEffects();
	}

	//Intended to be called from CharacterAnimationEvents
	public void PlayFootstep()
	{
		currentGroundEffects?.footstepSound?.Play(transform.position, soundType);
	}

	public void AddGroundTypeRegion(GroundTypeRegion region)
	{
		//Remove if already there and re-add to top of list
		if(groundTypeRegions.Contains(region))
			groundTypeRegions.Remove(region);

		groundTypeRegions.Add(region);

		UpdateGroundEffects();
	}

	public void RemoveGroundTypeRegion(GroundTypeRegion region)
	{
		if(groundTypeRegions.Contains(region))
			groundTypeRegions.Remove(region);

		UpdateGroundEffects();
	}

	private void UpdateGroundEffects()
	{
		GroundType groundType = null;

		if(groundTypeRegions.Count > 0)
		{
			//Always get from top of list (latest entered, allows regions inside regions)
			groundType = groundTypeRegions[groundTypeRegions.Count - 1].groundType;
		}
		else
		{
			if (SceneData.Instance)
				groundType = SceneData.Instance.defaultGroundType;
			else
				Debug.LogError("Could not find SceneData for default ground type!", this);
		}

		GroundEffects newGroundEffects;

		if (groundType != null && groundEffects.ContainsKey(groundType))
			newGroundEffects = groundEffects[groundType];
		else
			newGroundEffects = null;

		//Only switch effects if we've actually changed
		if(newGroundEffects != currentGroundEffects)
		{
			currentGroundEffects = newGroundEffects;

			currentTrailEffect?.StopParticles();
			currentTrailEffect = null;

			if(currentGroundEffects?.trailEffect?.gameObject)
			{
				GameObject obj = ObjectPooler.GetPooledObject(currentGroundEffects.trailEffect.gameObject);
				currentTrailEffect = obj.GetComponent<TrailParticles>();

				currentTrailEffect.StartParticles(transform, () =>
                {
                    bool isGrounded = characterMove.IsGrounded;
                    if (!isGrounded && didJustWallJump)
                    {
                        didJustWallJump = false;
                        isGrounded = true;
                    }

                    return isGrounded;
                });
			}
		}
	}
}
