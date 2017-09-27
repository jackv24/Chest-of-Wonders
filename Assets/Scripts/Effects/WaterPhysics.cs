using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
	public float spring = 0.02f;
	public float damping = 0.04f;
	public float spread = 0.05f;

	[Space()]
	public float velocityUpMultiplier = 1.0f;
	public float velocityDownMultiplier = 1.0f;

	[Space()]
	public float pushVelocity = 0.025f;
	public float pullVelocity = -0.0125f;

	public enum SplashType
	{
		Push,
		Pull
	}

	[Space()]
	public int resolution = 2;

	[Space()]
	public GameObject splashEffect;

	private float defaultParticleSpeed;
	private float defaultParticleEmission;

	public float particleHeight = -0.35f;

	public float baseSpeed = 1.0f;
	public float baseEmission = 5.0f;

	private Vector2[] positions;
	private float[] velocities;
	private float[] accelerations;

	private GameObject meshObject;
	private Mesh mesh;
	private Vector3[] vertices;

	private int edgeCount;
	private int nodeCount;

	private float top;
	private float bottom;
	private float left;

	private GameObject[] colliders;

	private LineRenderer lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		float width = transform.localScale.x;
		float height = transform.localScale.y;

		edgeCount = Mathf.RoundToInt(width) * resolution;
		nodeCount = edgeCount + 1;

		lineRenderer.numPositions = nodeCount;
		lineRenderer.useWorldSpace = true;

		top = transform.position.y + height / 2;
		bottom = transform.position.y - height / 2;
		left = transform.position.x - width / 2;

		//Setup arrays
		positions = new Vector2[nodeCount];
		velocities = new float[nodeCount];
		accelerations = new float[nodeCount];
		colliders = new GameObject[nodeCount];

		//Setup mesh
		meshObject = new GameObject("Mesh");
		meshObject.transform.SetParent(transform);
		meshObject.transform.localPosition = Vector3.zero;

		//Cache material for new mesh
		Material material = GetComponent<MeshRenderer>().sharedMaterial;

		//Create and assign new mesh to mesh object
		mesh = new Mesh();
		mesh.name = "Generated Water Mesh";
		meshObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		meshObject.AddComponent<MeshRenderer>().sharedMaterial = material;
		meshObject.GetComponent<MeshRenderer>().sortingLayerName = "Effects";

		//Destroy mesh and rendering comonents - used for preview purposes only
		Destroy(GetComponent<MeshFilter>());
		Destroy(GetComponent<MeshRenderer>());

		//Set array values
		for(int i = 0; i < nodeCount; i++)
		{
			positions[i] = new Vector2(left + width * (i / (float)edgeCount), top);
			accelerations[i] = 0;
			velocities[i] = 0;

			lineRenderer.SetPosition(i, new Vector3(positions[i].x, positions[i].y, transform.position.z));
		}

		//Create initial mesh data
		vertices = new Vector3[edgeCount * 4];
		Vector2[] uvs = new Vector2[edgeCount * 4];
		int[] indices = new int[edgeCount * 6];

		//Generate mesh
		for(int i = 0; i < edgeCount; i++)
		{
			int index = i * 4;

			float x1 = i > 0 ? (i - 1) / (float)edgeCount : 0;
			float x2 = i / (float)edgeCount;

			uvs[index] = new Vector2(x1, 1);
			uvs[index + 1] = new Vector2(x2, 1);
			uvs[index + 2] = new Vector2(x1, 0);
			uvs[index + 3] = new Vector2(x2, 0);

			int ind = i * 6;

			indices[ind] = index;
			indices[ind + 1] = index + 1;
			indices[ind + 2] = index + 3;
			indices[ind + 3] = index + 3;
			indices[ind + 4] = index + 2;
			indices[ind + 5] = index;
		}

		UpdateMesh();

		//Assign generated mesh values
		mesh.uv = uvs;
		mesh.triangles = indices;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		//Setup collisions
		for (int i = 0; i < nodeCount; i++)
		{
			colliders[i] = new GameObject("Trigger");
			colliders[i].AddComponent<BoxCollider2D>().isTrigger = true;
			colliders[i].transform.SetParent(transform);
			colliders[i].transform.position = new Vector3(left + width * i / edgeCount, top - 0.05f, transform.position.z);
			colliders[i].transform.localScale = new Vector3((width / edgeCount)/transform.localScale.x, 0.1f/transform.localScale.y, 1);
			colliders[i].AddComponent<WaterDetector>();
		}

		//Cache default particle values
		if (splashEffect)
		{
			ParticleSystem system = splashEffect.GetComponent<ParticleSystem>();

			if (system)
			{
				ParticleSystem.MainModule main = system.main;
				defaultParticleSpeed = main.startSpeedMultiplier;

				ParticleSystem.EmissionModule em = system.emission;
				defaultParticleEmission = em.rateOverTimeMultiplier;
			}
		}
	}

	void UpdateMesh()
	{
		for (int i = 0; i < edgeCount; i++)
		{
			int index = i * 4;

			//Update 4 vertices for every node
			vertices[index] = new Vector3(positions[i].x, positions[i].y, transform.position.z) - transform.position;
			vertices[index + 1] = new Vector3(positions[i + 1].x, positions[i + 1].y, transform.position.z) - transform.position;
			vertices[index + 2] = new Vector3(positions[i].x, bottom, transform.position.z) - transform.position;
			vertices[index + 3] = new Vector3(positions[i + 1].x, bottom, transform.position.z) - transform.position;
		}

		//Reassign vertex positions back to mesh
		mesh.vertices = vertices;
	}

	private void FixedUpdate()
	{
		//Move node physics
		for(int i = 0; i < positions.Length; i++)
		{
			float force = spring * (positions[i].y - top) + velocities[i] * damping;
			accelerations[i] = -force;
			positions[i].y += velocities[i];
			velocities[i] += accelerations[i];

			lineRenderer.SetPosition(i, new Vector3(positions[i].x, positions[i].y, transform.position.z));
		}

		///Propogate to neighbouring nodes
		float[] leftDeltas = new float[positions.Length];
		float[] rightDeltas = new float[positions.Length];

		//Loop 8 times - more fluid if done over time
		for (int j = 0; j < 8; j++)
		{
			for(int i = 0; i < positions.Length; i++)
			{
				if(i > 0)
				{
					leftDeltas[i] = spread * (positions[i].y - positions[i - 1].y);
					velocities[i - 1] += leftDeltas[i];
				}
				if(i < positions.Length - 1)
				{
					rightDeltas[i] = spread * (positions[i].y - positions[i + 1].y);
					velocities[i + 1] += rightDeltas[i];
				}
			}
		}

		//Apply movement data - done after propogation calculation
		for(int i = 0; i < positions.Length; i++)
		{
			if (i > 0)
				positions[i - 1].y += leftDeltas[i];

			if (i < positions.Length - 1)
				positions[i + 1].y += rightDeltas[i];
		}

		//Clamp positions to bottom
		for(int i = 0; i < positions.Length; i++)
		{
			if (positions[i].y < bottom)
				positions[i].y = bottom;
		}

		UpdateMesh();
	}

	public void Splash(float xPos, float velocity, SplashType splashType)
	{
		//Make sure splash is within bounds
		if(xPos >= positions[0].x && xPos <= positions[positions.Length - 1].x)
		{
			//Make xPos relative to water body
			xPos -= positions[0].x;

			//Calulate which node is closest
			int index = Mathf.RoundToInt((positions.Length - 1) * (xPos / (positions[positions.Length - 1].x - positions[0].x)));

			//Set velocity of node
			velocities[index] = velocity * (velocity > 0 ? velocityUpMultiplier : velocityDownMultiplier) + (splashType == SplashType.Push ? pushVelocity : pullVelocity);

			if (splashEffect)
			{
				//Splash particle effect
				GameObject obj = ObjectPooler.GetPooledObject(splashEffect);
				obj.transform.position = new Vector3(positions[index].x, positions[index].y + particleHeight, transform.position.z);

				ParticleSystem system = obj.GetComponent<ParticleSystem>();

				if(system)
				{
					float multiplier = 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);

					ParticleSystem.MainModule main = system.main;
					main.startSpeedMultiplier = defaultParticleSpeed * multiplier + baseSpeed;

					ParticleSystem.EmissionModule em = system.emission;
					em.rateOverTimeMultiplier = defaultParticleEmission * multiplier + baseEmission;

					system.Play();
				}
			}
		}
	}
}
