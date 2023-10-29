using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance { set; get; }

	private const bool SHOW_COLLIDER = true;

	//Level Spawning
	private const float DISTANCE_BEFORE_SPAWN = 100.0f;
	private const int INITIAL_SEGMENTS = 10;
	private const int MAX_SEGMENTS_ON_SCREEN = 15;
	private Transform cameraContainer;
	private int amountOfActiveSegments;
	private int continiousSegments;
	private int currentSpawnZ;
	private int currentLevel;
	private int y1, y2, y3;


	//list of pieces
	public List<Piece> ramps = new List<Piece>();
	public List<Piece> londblocks = new List<Piece>();
	public List<Piece> jumps = new List<Piece>();
	public List<Piece> slides = new List<Piece>();

	[HideInInspector]
	public List<Piece> pieces = new List<Piece>(); // all the pieces in one pool

	//LIST OF SEGMENTS
	public List<Segment> availableSegments = new List<Segment>();
	public List<Segment> availableTransitions = new List<Segment>();

	[HideInInspector]
	public List<Segment> segments = new List<Segment>();

	//gameplay
	private bool isMoving = false;

	private void Awake()
	{
		Instance = this;
		cameraContainer = Camera.main.transform;
		currentSpawnZ = 0;
		currentLevel = 0;

	}

	private void Start()
	{
		for (int i = 0; i < INITIAL_SEGMENTS; i++)
		{
			GenerateSegment();
		}

	}

	private void GenerateSegment()
	{
		SpawnSegment();

		if(Random.Range(0f, 1f) < (continiousSegments * .25f))
		{
			//spawn transition seg
			continiousSegments = 0;
			SpawnTransition();
		}
		else
		{
			continiousSegments++;
		}
	}

	private void SpawnSegment()
	{
		List<Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
		int id = Random.Range(0, possibleSeg.Count);

		Segment segment = GetSegment(id, false);

		y1 = segment.endY1;
		y2 = segment.endY2;
		y3 = segment.endY3;

		segment.transform.SetParent(transform);
		segment.transform.localPosition = Vector3.forward * currentSpawnZ;

		currentSpawnZ += segment.lenght;
		amountOfActiveSegments++;
		segment.Spawn();


	}

	private void SpawnTransition()
	{
		List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
		int id = Random.Range(0, possibleTransition.Count);

		Segment segment = GetSegment(id, true);

		y1 = segment.endY1;
		y2 = segment.endY2;
		y3 = segment.endY3;

		segment.transform.SetParent(transform);
		segment.transform.localPosition = Vector3.forward * currentSpawnZ;

		currentSpawnZ += segment.lenght;
		amountOfActiveSegments++;
		segment.Spawn();
	}

	public Segment GetSegment(int id, bool transition)
	{
		Segment segment = null;
		segment = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);

		if(segment == null)
		{
			GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
			segment = go.GetComponent<Segment>();

			segment.SegId = id;
			segment.transition = transition;

			segments.Insert(0, segment);
		}
		else
		{
			segments.Remove(segment);
			segments.Insert(0, segment);
		}

		return segment;
	}

	public Piece GetPiece(PieceType pt, int visualIndex)
	{
		Piece piece = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);
		if (piece == null)
		{
			GameObject go = null;

			if(pt == PieceType.ramp)
			{
				go = ramps[visualIndex].gameObject;
			}
			else if(pt == PieceType.longblock)
			{
				go = londblocks[visualIndex].gameObject;
			}
			else if (pt == PieceType.jump)
			{
				go = jumps[visualIndex].gameObject;
			}
			else if (pt == PieceType.slide)
			{
				go = slides[visualIndex].gameObject;
			}

			go = Instantiate(go);
			piece = go.GetComponent<Piece>();
			pieces.Add(piece);
		}

		return piece;
	}
}
