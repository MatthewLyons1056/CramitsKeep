using UnityEngine;
using System.Collections;

namespace Pathfinding {
	[UniqueComponent(tag = "ai.destination")]
	public class AIDestinationSetter : VersionedMonoBehaviour {
		/// <summary>The object that the AI should move to</summary>
		[Tooltip("Set to target Self")]
		public Transform self;
		[Tooltip("Set to target Player")]
		public Transform playerTarget;
		public Transform flexTransform;
		[Tooltip("Range of which target will agro")]
		public float agroRange = 5;

		IAstarAI ai;
		public void Start()
        {
			//get player
			GameObject player = GameObject.Find("Player");
			playerTarget = player.transform;
			flexTransform = self;
        }
		void OnEnable () {
			ai = GetComponent<IAstarAI>();
			if (ai != null) ai.onSearchPath += Update;
		}
		void OnDisable () {
			if (ai != null) ai.onSearchPath -= Update;
		}
		/// <summary>Updates the AI's destination every frame</summary>
		void Update () {
			if (flexTransform != null && ai != null) ai.destination = flexTransform.position;
			//Debug.Log(flexTransform);
			//Difference();
		}
	}
}
