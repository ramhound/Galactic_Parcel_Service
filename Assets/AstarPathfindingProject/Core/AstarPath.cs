using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

#if NETFX_CORE
using Thread = Pathfinding.WindowsStore.Thread;
using ParameterizedThreadStart = Pathfinding.WindowsStore.ParameterizedThreadStart;
#else
using Thread = System.Threading.Thread;
using ParameterizedThreadStart = System.Threading.ParameterizedThreadStart;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("Pathfinding/Pathfinder")]
/**
 * Core component for the A* Pathfinding System.
 * This class handles all of the pathfinding system, calculates all paths and stores the info.\n
 * This class is a singleton class, meaning there should only exist at most one active instance of it in the scene.\n
 * It might be a bit hard to use directly, usually interfacing with the pathfinding system is done through the Seeker class.
 *
 * \nosubgrouping
 * \ingroup relevant
 */
public class AstarPath : MonoBehaviour {

	/** The version number for the A* %Pathfinding Project */
	public static System.Version Version {
		get {
			return new System.Version (3,7,5);
		}
	}

	/** Information about where the package was downloaded */
	public enum AstarDistribution { WebsiteDownload, AssetStore };

	/** Used by the editor to guide the user to the correct place to download updates */
	public static readonly AstarDistribution Distribution = AstarDistribution.WebsiteDownload;

	/** Which branch of the A* %Pathfinding Project is this release.
	 * Used when checking for updates so that
	 * users of the development versions can get notifications of development
	 * updates.
	 */
	public static readonly string Branch = "master_Pro";

	/** Used by the editor to show some Pro specific stuff.
	 * Note that setting this to true will not grant you any additional features */
	public static readonly bool HasPro = true;

	/** See Pathfinding.AstarData
	 * \deprecated
	 */
	[System.Obsolete]
	public System.Type[] graphTypes {
		get {
			return astarData.graphTypes;
		}
	}

	/** Holds all graph data */
	public AstarData astarData;

	/** Returns the active AstarPath object in the scene.
	 * \note This is only set if the AstarPath object has been initialized (which happens in Awake).
	 */
#if UNITY_4_6 || UNITY_4_3
	public static new AstarPath active;
#else
	public static AstarPath active;
#endif

	/** Shortcut to Pathfinding.AstarData.graphs */
	public NavGraph[] graphs {
		get {
			if (astarData == null)
				astarData = new AstarData ();
			return astarData.graphs;
		}
		set {
			if (astarData == null)
				astarData = new AstarData ();
			astarData.graphs = value;
		}
	}

#region InspectorDebug
	/** @name Inspector - Debug
	 * @{ */

	/** Toggle for showing the gizmo debugging for the graphs in the scene view (editor only). */
	public bool showNavGraphs = true;

	/** Toggle to show unwalkable nodes.
	 *
	 * \note Only relevant in the editor
	 *
	 * \see unwalkableNodeDebugSize
	 */
	public bool showUnwalkableNodes = true;

	/** The mode to use for drawing nodes in the sceneview.
	 *
	 * \note Only relevant in the editor
	 *
	 * \see Pathfinding.GraphDebugMode
	 */
	public GraphDebugMode debugMode;

	/** Low value to use for certain #debugMode modes.
	 * For example if #debugMode is set to G, this value will determine when the node will be totally red.
	 *
	 * \note Only relevant in the editor
	 * \see #debugRoof
	 */
	public float debugFloor = 0;

	/** High value to use for certain #debugMode modes.
	 * For example if #debugMode is set to G, this value will determine when the node will be totally green.
	 *
	 * For the penalty debug mode, the nodes will be colored green when they have a penalty of zero and red
	 * when their penalty is greater or equal to this value and something between red and green for values in between.
	 *
	 * \note Only relevant in the editor
	 *
	 * \see #debugFloor
	 */
	public float debugRoof = 20000;

	/** If set, the #debugFloor and #debugRoof values will not be automatically recalculated.
	 *
	 * \note Only relevant in the editor
	 */
	public bool manualDebugFloorRoof = false;


	/** If enabled, nodes will draw a line to their 'parent'.
	 * This will show the search tree for the latest path.
	 *
	 * \note Only relevant in the editor
	 *
	 * \todo Add a showOnlyLastPath flag to indicate whether to draw every node or only the ones visited by the latest path.
	 */
	public bool	showSearchTree = false;

	/** Size of the red cubes shown in place of unwalkable nodes.
	 *
	 * \note Only relevant in the editor
	 * \see showUnwalkableNodes */
	public float unwalkableNodeDebugSize = 0.3F;

	/** The amount of debugging messages.
	 * Use less debugging to improve performance (a bit) or just to get rid of the Console spamming.\n
	 * Use more debugging (heavy) if you want more information about what the pathfinding is doing.\n
	 * InGame will display the latest path log using in game GUI.
	 */
	public PathLog logPathResults = PathLog.Normal;

	/** @} */
#endregion

#region InspectorSettings
	/** @name Inspector - Settings
	 * @{ */

	/** Max Nearest Node Distance.
	 * When searching for a nearest node, this is the limit (world units) for how far away it is allowed to be.
	 * \see Pathfinding.NNConstraint.constrainDistance
	 */
	public float maxNearestNodeDistance = 100;

	/** Max Nearest Node Distance Squared.
	 * \see #maxNearestNodeDistance */
	public float maxNearestNodeDistanceSqr {
		get { return maxNearestNodeDistance*maxNearestNodeDistance; }
	}

	/** If true, all graphs will be scanned in Awake.
	 * This does not include loading from the cache.
	 * If you disable this, you will have to call \link Scan AstarPath.active.Scan () \endlink yourself to enable pathfinding.
	 * Alternatively you could load a saved graph from a file.
	 */
	public bool scanOnStartup = true;

	/** Do a full GetNearest search for all graphs.
	 * Additional searches will normally only be done on the graph which in the first fast search seemed to have the closest node.
	 * With this setting on, additional searches will be done on all graphs since the first check is not always completely accurate.\n
	 * More technically: GetNearestForce on all graphs will be called if true, otherwise only on the one graph which's GetNearest search returned the best node.\n
	 * Usually faster when disabled, but higher quality searches when enabled.
	 * When using a a navmesh or recast graph, for best quality, this setting should be combined with the Pathfinding.NavMeshGraph.accurateNearestNode setting set to true.
	 * \note For the PointGraph this setting doesn't matter much as it has only one search mode.
	 */
	public bool fullGetNearestSearch = false;

	/** Prioritize graphs.
	 * Graphs will be prioritized based on their order in the inspector.
	 * The first graph which has a node closer than #prioritizeGraphsLimit will be chosen instead of searching all graphs.
	 */
	public bool prioritizeGraphs = false;

	/** Distance limit for #prioritizeGraphs.
	 * \see #prioritizeGraphs
	 */
	public float prioritizeGraphsLimit = 1F;

	/** Reference to the color settings for this AstarPath object.
	 * Color settings include for example which color the nodes should be in, in the sceneview. */
	public AstarColor colorSettings;

	/** Stored tag names.
	 * \see AstarPath.FindTagNames
	 * \see AstarPath.GetTagNames
	 */
	[SerializeField]
	protected string[] tagNames = null;

	/** The heuristic to use.
	 * The heuristic, often referred to as 'H' is the estimated cost from a node to the target.
	 * Different heuristics affect how the path picks which one to follow from multiple possible with the same length
	 * \see Pathfinding.Heuristic
	 */
	public Heuristic heuristic = Heuristic.Euclidean;

	/** The scale of the heuristic. If a smaller value than 1 is used, the pathfinder will search more nodes (slower).
	 * If 0 is used, the pathfinding will be equal to dijkstra's algorithm.
	 * If a value larger than 1 is used the pathfinding will (usually) be faster because it expands fewer nodes, but the paths might not longer be optimal
	 */
	public float heuristicScale = 1F;

	/** Number of pathfinding threads to use.
	 * Multithreading puts pathfinding in another thread, this is great for performance on 2+ core computers since the framerate will barely be affected by the pathfinding at all.
	 * - None indicates that the pathfinding is run in the Unity thread as a coroutine
	 * - Automatic will try to adjust the number of threads to the number of cores and memory on the computer.
	 * 	Less than 512mb of memory or a single core computer will make it revert to using no multithreading.
	 *
	 * It is recommended that you use one of the "Auto" settings that are available.
	 * The reason is that even if your computer might be beefy and have 8 cores.
	 * Other computers might only be quad core or dual core in which case they will not benefit from more than
	 * 1 or 3 threads respectively (you usually want to leave one core for the unity thread).
	 * If you use more threads than the number of cores on the computer it is mostly just wasting memory, it will not run any faster.
	 * The extra memory usage is not trivially small. Each thread needs to keep a small amount of data for each node in all the graphs.
	 * It is not the full graph data but it is proportional to the number of nodes.
	 * The automatic settings will inspect the machine it is running on and use that to determine the number of threads so that no memory is wasted.
	 *
	 * The exception is if you only have one (or maybe two characters) active at time. Then you should probably just go with one thread always since it is very unlikely
	 * that you will need the extra throughput given by more threads. Keep in mind that more threads primarily increases throughput by calculating different paths on different
	 * threads, it will not calculate individual paths any faster.
	 *
	 * Note that if you are modifying the pathfinding core scripts or if you are directly modifying graph data without using any of the
	 * safe wrappers (like RegisterSafeUpdate) multithreading can cause strange errors and pathfinding stopping to work if you are not careful.
	 * For basic usage (not modding the pathfinding core) it should be safe.\n
	 *
	 * \note WebGL does not support threads at all (since javascript is single-threaded)
	 *
	 * \see CalculateThreadCount
	 *
	 * \astarpro
	 */
	public ThreadCount threadCount = ThreadCount.None;

	/** Max number of milliseconds to spend each frame for pathfinding.
	 * At least 500 nodes will be searched each frame (if there are that many to search).
	 * When using multithreading this value is quite irrelevant,
	 * but do not set it too low since that could add upp to some overhead, 10ms will work good for multithreading */
	public float maxFrameTime = 1F;

	/** Defines the minimum amount of nodes in an area.
	 * If an area has less than this amount of nodes, the area will be flood filled again with the area ID GraphNode.MaxAreaIndex-1,
	 * it shouldn't affect pathfinding in any significant way.\n
	 * If you want to be able to separate areas from one another for some reason (for example to do a fast check to see if a path is at all possible)
	 * you should set this variable to 0.\n
	  * Can be found in A* Inspector-->Settings-->Min Area Size
	  *
	  * \version Since version 3.6, this variable should in most cases be set to 0 since the max number of area indices available has been greatly increased.
	  */
	public int minAreaSize = 0;

	/** Limit graph updates.
	 * If toggled, graph updates will be executed less often (specified by #maxGraphUpdateFreq)
	 */
	public bool limitGraphUpdates = true;

	/** How often should graphs be updated.
	 * If #limitGraphUpdates is true, this defines the minimum amount of seconds between each graph update.
	 */
	public float maxGraphUpdateFreq = 0.2F;

	/** @} */
#endregion

#region DebugVariables
	/** @name Debug Members
	 * @{ */

#if ProfileAstar
	/** How many paths has been computed this run. From application start.\n
	 * Debugging variable
	 */
	public static int PathsCompleted = 0;

	public static System.Int64 				TotalSearchedNodes = 0;
	public static System.Int64			 	TotalSearchTime = 0;
#endif

	/** The time it took for the last call to Scan() to complete.
	 * Used to prevent automatically rescanning the graphs too often (editor only)
	 */
	public float lastScanTime {get; private set;}

	/** The path to debug using gizmos.
	 * This is equal to the last path which was calculated.
	 * It is used in the editor to draw debug information using gizmos.
	 */
	[System.NonSerialized]
	public Path debugPath;

	/** NodeRunData from #debugPath.
	 * Returns null if #debugPath is null
	 */
	public PathHandler debugPathData {
		get {
			if (debugPath == null) return null;
			return debugPath.pathHandler;
		}
	}

	/** Debug string from the last completed path.
	 * Will be updated if #logPathResults == PathLog.InGame
	 */
	string inGameDebugPath;

	/* @} */
#endregion

#region StatusVariables

	/** Set when scanning is being done. It will be true up until the FloodFill is done.
	 * Used to better support Graph Update Objects called for example in OnPostScan */
	public bool isScanning {get; private set;}

	/** Number of parallel pathfinders.
	 * Returns the number of concurrent processes which can calculate paths at once.
	 * When using multithreading, this will be the number of threads, if not using multithreading it is always 1 (since only 1 coroutine is used).
	 * \see threadInfos
	 * \see IsUsingMultithreading
	 */
	public int NumParallelThreads {
		get {
			return pathProcessor.NumThreads;
		}
	}

	/** Returns whether or not multithreading is used.
	 * \exception System.Exception Is thrown when it could not be decided if multithreading was used or not.
	 * This should not happen if pathfinding is set up correctly.
	 * \note This uses info about if threads are running right now, it does not use info from the settings on the A* object.
	 */
	public bool IsUsingMultithreading {
		get {
			return pathProcessor.IsUsingMultithreading;
		}
	}

	/** Returns if any graph updates are waiting to be applied.
	 * \deprecated Use IsAnyGraphUpdateQueued instead
	 */
	[System.Obsolete("Fixed grammar, use IsAnyGraphUpdateQueued instead")]
	public bool IsAnyGraphUpdatesQueued { get { return IsAnyGraphUpdateQueued; }}

	/** Returns if any graph updates are waiting to be applied */
	public bool IsAnyGraphUpdateQueued { get { return graphUpdates.IsAnyGraphUpdateQueued; }}

#endregion

#region Callbacks
	/** @name Callbacks */
	 /* Callbacks to pathfinding events.
	 * These allow you to hook in to the pathfinding process.\n
	 * Callbacks can be used like this:
	 * \code
	 * public void Start () {
	 * 	AstarPath.OnPostScan += SomeFunction;
	 * }
	 *
	 * public void SomeFunction (AstarPath active) {
	 * 	//This will be called every time the graphs are scanned
	 * }
	 * \endcode
	*/
	 /** @{ */

	/** Called on Awake before anything else is done.
	  * This is called at the start of the Awake call, right after #active has been set, but this is the only thing that has been done.\n
	  * Use this when you want to set up default settings for an AstarPath component created during runtime since some settings can only be changed in Awake
	  * (such as multithreading related stuff)
	  * \code
	  * //Create a new AstarPath object on Start and apply some default settings
	  * public void Start () {
	  * 	AstarPath.OnAwakeSettings += ApplySettings;
	  * 	AstarPath astar = AddComponent<AstarPath>();
	  * }
	  *
	  * public void ApplySettings () {
	  * 	//Unregister from the delegate
	  * 	AstarPath.OnAwakeSettings -= ApplySettings;
	  *
	  * 	//For example useMultithreading should not be changed after the Awake call
	  * 	//so here's the only place to set it if you create the component during runtime
	  * 	AstarPath.active.useMultithreading = true;
	  * }
	  * \endcode
	  */
	public static System.Action OnAwakeSettings;

	/** Called for each graph before they are scanned */
	public static OnGraphDelegate OnGraphPreScan;

	/** Called for each graph after they have been scanned. All other graphs might not have been scanned yet. */
	public static OnGraphDelegate OnGraphPostScan;

	/** Called for each path before searching. Be careful when using multithreading since this will be called from a different thread. */
	public static OnPathDelegate OnPathPreSearch;

	/** Called for each path after searching. Be careful when using multithreading since this will be called from a different thread. */
	public static OnPathDelegate OnPathPostSearch;

	/** Called before starting the scanning */
	public static OnScanDelegate OnPreScan;

	/** Called after scanning. This is called before applying links, flood-filling the graphs and other post processing. */
	public static OnScanDelegate OnPostScan;

	/** Called after scanning has completed fully. This is called as the last thing in the Scan function. */
	public static OnScanDelegate OnLatePostScan;

	/** Called when any graphs are updated. Register to for example recalculate the path whenever a graph changes. */
	public static OnScanDelegate OnGraphsUpdated;

	/**
	 * Called when \a pathID overflows 65536.
	 * The Pathfinding.CleanupPath65K will be added to the queue, and directly after, this callback will be called.
	 * \note This callback will be cleared every time it is called, so if you want to register to it repeatedly, register to it directly on receiving the callback as well.
	 */
	public static System.Action On65KOverflow;

	/**
	 * Will send a callback when it is safe to update the nodes. Register to this with RegisterThreadSafeNodeUpdate
	 * When it is safe is defined as between the path searches.
	 * This callback will only be sent once and is nulled directly after the callback is sent.
	 */
	private static System.Action OnThreadSafeCallback;

	/**
	 * Used internally to enable gizmos in editor scripts.
	 * \warning Used internally by the editor, do not use this in your code
	 */
	public System.Action OnDrawGizmosCallback;

	/**
	 * Used internally to unload temporary meshes in the editor.
	 * \warning Used internally by the editor, do not use this in your code
	 */
	public System.Action OnUnloadGizmoMeshes;

	/** \deprecated */
	[System.ObsoleteAttribute]
	public System.Action OnGraphsWillBeUpdated;

	/** \deprecated */
	[System.ObsoleteAttribute]
	public System.Action OnGraphsWillBeUpdated2;

	/* @} */
#endregion

#region MemoryStructures

	/** Processes graph updates */
	readonly GraphUpdateProcessor graphUpdates;

	/** Processes work items */
	readonly WorkItemProcessor workItems;

	/** Holds all paths waiting to be calculated and calculates them */
	PathProcessor pathProcessor;

	bool graphUpdateRoutineRunning = false;

	/** Makes sure QueueGraphUpdates will not queue multiple graph update orders */
	bool graphUpdatesWorkItemAdded = false;

	/** Time the last graph update was done.
	 * Used to group together frequent graph updates to batches
	 */
	float lastGraphUpdate = -9999F;

	/** True if any work items are currently queued */
	bool workItemsQueued = false;

	/** Holds all completed paths waiting to be returned to where they were requested */
	readonly PathReturnQueue pathReturnQueue;

	/** Holds settings for heuristic optimization.
	 * \see heuristic-opt
	 *
	 * \astarpro
	 */
	public EuclideanEmbedding euclideanEmbedding = new EuclideanEmbedding();

#endregion

	/** Shows or hides graph inspectors.
	 * Used internally by the editor
	 */
	public bool showGraphs = false;

#region ThreadingMembers

	private static readonly System.Object safeUpdateLock = new object();

#endregion

	/** The next unused Path ID.
	 * Incremented for every call to GetNextPathID
	 */
	private ushort nextFreePathID = 1;

	private AstarPath () {
		// Make sure that the pathProcessor is never null
		pathProcessor = new PathProcessor(this, pathReturnQueue, 0, true);

		pathReturnQueue = new PathReturnQueue(this);
		workItems = new WorkItemProcessor(this);
		graphUpdates = new GraphUpdateProcessor(this);

		// Forward graphUpdates.OnGraphsUpdated to AstarPath.OnGraphsUpdated
		graphUpdates.OnGraphsUpdated += () => {
			if (OnGraphsUpdated != null) {
				OnGraphsUpdated(this);
			}
		};
	}

	/** Returns tag names.
	 * Makes sure that the tag names array is not null and of length 32.
	 * If it is null or not of length 32, it creates a new array and fills it with 0,1,2,3,4 etc...
	 * \see AstarPath.FindTagNames
	 */
	public string[] GetTagNames () {
		if (tagNames == null || tagNames.Length	!= 32) {
			tagNames = new string[32];
			for (int i=0;i<tagNames.Length;i++) {
				tagNames[i] = ""+i;
			}
			tagNames[0] = "Basic Ground";
		}
		return tagNames;
	}

	/** Tries to find an AstarPath object and return tag names.
	 * If an AstarPath object cannot be found, it returns an array of length 1 with an error message.
	 * \see AstarPath.GetTagNames
	 */
	public static string[] FindTagNames () {
		if (active != null) {
			return active.GetTagNames ();
		} else {
			AstarPath astar = GameObject.FindObjectOfType<AstarPath>();
			if (astar != null) {
				active = astar;
				return astar.GetTagNames ();
			} else {
				return new string[1] {"There is no AstarPath component in the scene"};
			}
		}
	}

	/** Returns the next free path ID.
	 *
	 * If the next free path ID overflows 65535, a cleanup operation is queued
	 * \see Pathfinding.CleanupPath65K
	 */
	internal ushort GetNextPathID () {
		if (nextFreePathID == 0) {
			nextFreePathID++;

			//Queue a cleanup operation to zero all path IDs
			//StartPath (new CleanupPath65K ());
			Debug.Log ("65K cleanup");

			//ushort toBeReturned = nextFreePathID;

			if (On65KOverflow != null) {
				System.Action tmp = On65KOverflow;
				On65KOverflow = null;
				tmp ();
			}

			//return nextFreePathID++;
		}
		return nextFreePathID++;
	}

#if !PhotonImplementation
	void RecalculateDebugLimits () {
		debugFloor = float.PositiveInfinity;
		debugRoof = float.NegativeInfinity;

		for (int i=0; i<graphs.Length; i++) {
			if (graphs[i] != null && graphs[i].drawGizmos) {
				graphs[i].GetNodes ((GraphNode node) => {

					if (!showSearchTree || debugPathData == null || NavGraph.InSearchTree(node,debugPath)) {
						var rnode = debugPathData != null ? debugPathData.GetPathNode(node) : null;
						if (rnode != null || debugMode == GraphDebugMode.Penalty) {
							switch (debugMode) {
							case GraphDebugMode.F:
								debugFloor = Mathf.Min (debugFloor, rnode.F);
								debugRoof = Mathf.Max (debugRoof, rnode.F);
								break;
							case GraphDebugMode.G:
								debugFloor = Mathf.Min (debugFloor, rnode.G);
								debugRoof = Mathf.Max (debugRoof, rnode.G);
								break;
							case GraphDebugMode.H:
								debugFloor = Mathf.Min (debugFloor, rnode.H);
								debugRoof = Mathf.Max (debugRoof, rnode.H);
								break;
							case GraphDebugMode.Penalty:
								debugFloor = Mathf.Min (debugFloor, node.Penalty);
								debugRoof = Mathf.Max (debugRoof, node.Penalty);
								break;
							}
						}
					}
					return true;
				});
			}
		}

		if (float.IsInfinity (debugFloor)) {
			debugFloor = 0;
			debugRoof = 1;
		}

		// Make sure they are not identical, that will cause the color interpolation to fail
		if (debugRoof-debugFloor < 1) debugRoof += 1;
	}

	/** Calls OnDrawGizmos on graph generators and also #OnDrawGizmosCallback */
	private void OnDrawGizmos () {
		// OnDrawGizmos may be called from EditorUtility.DisplayProgressBar
		// which is called repeatedly while the graphs are scanned in the
		// editor. When this happens we want to avoid drawing anything
		// since the graphs may not contain valid information
		// Also relevant if graphs would be scanned asynchronously
		// which is not possible right now, but may be in the future
		if (isScanning) {
			return;
		}

		// Make sure the singleton pattern holds
		// Might not hold if the Awake method
		// has not been called yet
		if (active == null) {
			active = this;
		} else if (active != this) {
			return;
		}

		if (graphs == null) return;

		// If updating graphs, graph info might not be valid right now
		if (workItems.workItemsInProgress) return;

		AstarProfiler.StartProfile ("OnDrawGizmos");

		if (showNavGraphs && !manualDebugFloorRoof) {
			RecalculateDebugLimits();
		}

		// Loop through all graphs and draw their gizmos
		for (int i = 0; i < graphs.Length; i++) {
			if (graphs[i] != null && graphs[i].drawGizmos)
				graphs[i].OnDrawGizmos (showNavGraphs);
		}

		if (showNavGraphs) {
			euclideanEmbedding.OnDrawGizmos ();

			if (showUnwalkableNodes) {
				Gizmos.color = AstarColor.UnwalkableNode;

				GraphNodeDelegateCancelable del = DrawUnwalkableNode;

				for (int i = 0; i < graphs.Length; i++) {
					if (graphs[i] != null && graphs[i].drawGizmos) graphs[i].GetNodes (del);
				}
			}
		}

		if (OnDrawGizmosCallback != null) {
			OnDrawGizmosCallback ();
		}

		AstarProfiler.EndProfile ("OnDrawGizmos");
	}

	/** Draws a cube at the node's position if unwalkable.
	 * Used for gizmo drawing
	 */
	private bool DrawUnwalkableNode (GraphNode node) {
		if (!node.Walkable) {
			Gizmos.DrawCube ((Vector3)node.position, Vector3.one*unwalkableNodeDebugSize);
		}
		return true;
	}

#if !ASTAR_NO_GUI
	/** Draws the InGame debugging (if enabled), also shows the fps if 'L' is pressed down.
	 * \see #logPathResults PathLog
	 */
	private void OnGUI () {
		if (logPathResults == PathLog.InGame && inGameDebugPath != "") {
			GUI.Label (new Rect (5,5,400,600), inGameDebugPath);
		}
	}
	#endif
#endif

#line hidden
	/** Logs a string while taking into account #logPathResults */
	internal void Log (string s) {
		if (System.Object.ReferenceEquals(active,null)) {
			Debug.Log ("No AstarPath object was found : " + s);
			return;
		}

		if (active.logPathResults != PathLog.None && active.logPathResults != PathLog.OnlyErrors) {
			Debug.Log (s);
		}
	}
#line default

	/** Prints path results to the log. What it prints can be controled using #logPathResults.
	 * \see #logPathResults
	 * \see PathLog
	 * \see Pathfinding.Path.DebugString
	 */
	private void LogPathResults (Path p) {
		if (logPathResults == PathLog.None || (logPathResults == PathLog.OnlyErrors && !p.error)) {
			return;
		}

		string debug = p.DebugString (logPathResults);

		if (logPathResults == PathLog.InGame) {
			inGameDebugPath = debug;
		} else {
			Debug.Log (debug);
		}
	}

	/**
	 * Checks if any work items need to be executed
	 * then runs pathfinding for a while (if not using multithreading because
	 * then the calculation happens in other threads)
	 * and then returns any calculated paths to the
	 * scripts that requested them.
	 *
	 * \see PerformBlockingActions
	 * \see PathProcessor.TickNonMultithreaded
	 * \see PathReturnQueue.ReturnPaths
	 */
	private void Update () {
		// This class uses the [ExecuteInEditMode] attribute
		// So Update is called even when not playing
		// Don't do anything when not in play mode
		if (!Application.isPlaying) return;

		// Execute blocking actions such as graph updates
		// when not scanning
		if (!isScanning) {
			PerformBlockingActions();
		}

		// Calculates paths when not using multithreading
		pathProcessor.TickNonMultithreaded();

		// Return calculated paths
		pathReturnQueue.ReturnPaths(true);
	}

	private void PerformBlockingActions (bool force = false, bool unblockOnComplete = true) {
		if (pathProcessor.queue.AllReceiversBlocked) {
			// Return all paths before starting blocking actions
			// since these might change the graph and make returned paths invalid (at least the nodes)
			pathReturnQueue.ReturnPaths (false);

			// This must be called before since otherwise ProcessWorkItems might start pathfinding again
			// if no work items are left to be processed resulting in thread safe callbacks never being called
			if (OnThreadSafeCallback != null) {
				System.Action tmp = OnThreadSafeCallback;
				OnThreadSafeCallback = null;
				tmp ();
			}

			// Make sure that all receivers are still blocked (the callback could have started them)
			if (pathProcessor.queue.AllReceiversBlocked) {
				if (workItems.ProcessWorkItems (force)) {
					//At this stage there are no more work items, restart pathfinding threads
					workItemsQueued = false;

					if (unblockOnComplete) {
						// Recalculate
						if ( euclideanEmbedding.dirty ) {
							euclideanEmbedding.RecalculateCosts ();
						}

						pathProcessor.queue.Unblock();
					}
				}
			}
		}

	}

	/** Call during work items to queue a flood fill.
	 * \deprecated This method has been moved. Use the method on the context object that can be sent with work item delegates instead
	 *
	 * \see IWorkItemContext
	 */
	[System.Obsolete("This method has been moved. Use the method on the context object that can be sent with work item delegates instead")]
	public void QueueWorkItemFloodFill () {
		throw new System.Exception("This method has been moved. Use the method on the context object that can be sent with work item delegates instead");
	}

	/** If a WorkItem needs to have a valid flood fill during execution, call this method to ensure there are no pending flood fills.
	 * \deprecated This method has been moved. Use the method on the context object that can be sent with work item delegates instead
	 *
	 * \see IWorkItemContext
	 */
	[System.Obsolete("This method has been moved. Use the method on the context object that can be sent with work item delegates instead")]
	public void EnsureValidFloodFill () {
		throw new System.Exception("This method has been moved. Use the method on the context object that can be sent with work item delegates instead");
	}

	/** Add a work item to be processed when pathfinding is paused.
	 *
	 * \see ProcessWorkItems
	 */
	public void AddWorkItem (AstarWorkItem itm) {
		workItems.AddWorkItem(itm);

		// Make sure pathfinding is stopped and work items are processed
		if (!workItemsQueued) {
			workItemsQueued = true;
			if (!isScanning) {
				InterruptPathfinding ();
			}
		}

#if UNITY_EDITOR
		// If not playing, execute instantly
		if (!Application.isPlaying) {
			FlushWorkItems();
		}
#endif
	}

#region GraphUpdateMethods

	/** Will apply queued graph updates as soon as possible, regardless of #limitGraphUpdates.
	 * Calling this multiple times will not create multiple callbacks.
	 * Makes sure DoUpdateGraphs is called as soon as possible.\n
	 * This function is useful if you are limiting graph updates, but you want a specific graph update to be applied as soon as possible regardless of the time limit.
	 * \see FlushGraphUpdates
	 */
	public void QueueGraphUpdates () {
		if (!graphUpdatesWorkItemAdded) {
			graphUpdatesWorkItemAdded = true;
			var workItem = graphUpdates.GetWorkItem();

			// Add a new work item which first
			// sets the graphUpdatesWorkItemAdded flag to false
			// and then processes the graph updates
			AddWorkItem(new AstarWorkItem(() => {
				graphUpdatesWorkItemAdded = false;

				// Set the debugPath to null before
				// starting graph updates.
				// When updating graphs nodes might
				// be destroyed and then the path
				// data might not be valid anymore
				debugPath = null;

				workItem.init();
			}, workItem.update));
		}
	}

	/** Waits a moment with updating graphs.
	 * If limitGraphUpdates is set, we want to keep some space between them to let pathfinding threads running and then calculate all queued calls at once
	 */
	IEnumerator DelayedGraphUpdate () {
		graphUpdateRoutineRunning = true;

		yield return new WaitForSeconds (maxGraphUpdateFreq-(Time.time-lastGraphUpdate));
		QueueGraphUpdates ();
		graphUpdateRoutineRunning = false;
	}

	/** Update all graphs within \a bounds after \a t seconds.
	 * This function will add a GraphUpdateObject to the #graphUpdateQueue.
	 * The graphs will be updated as soon as possible.
	 */
	public void UpdateGraphs (Bounds bounds, float t) {
		UpdateGraphs (new GraphUpdateObject (bounds),t);
	}

	/** Update all graphs using the GraphUpdateObject after \a t seconds.
	 * This can be used to, e.g make all nodes in an area unwalkable, or set them to a higher penalty.
	*/
	public void UpdateGraphs (GraphUpdateObject ob, float t) {
		StartCoroutine (UpdateGraphsInteral (ob,t));
	}

	/** Update all graphs using the GraphUpdateObject after \a t seconds */
	IEnumerator UpdateGraphsInteral (GraphUpdateObject ob, float t) {
		yield return new WaitForSeconds (t);
		UpdateGraphs (ob);
	}

	/** Update all graphs within \a bounds.
	 * This function will add a GraphUpdateObject to the #graphUpdateQueue.
	 * The graphs will be updated as soon as possible.
	 *
	 * This is equivalent to\n
	 * UpdateGraphs (new GraphUpdateObject (bounds))
	 *
	 * \see FlushGraphUpdates
	 */
	public void UpdateGraphs (Bounds bounds) {
		UpdateGraphs (new GraphUpdateObject (bounds));
	}

	/** Update all graphs using the GraphUpdateObject.
	 * This can be used to, e.g make all nodes in an area unwalkable, or set them to a higher penalty.
	 * The graphs will be updated as soon as possible (with respect to #limitGraphUpdates)
	 *
	 * \see FlushGraphUpdates
	*/
	public void UpdateGraphs (GraphUpdateObject ob) {
		graphUpdates.UpdateGraphs(ob);

		// If we should limit graph updates, start a coroutine which waits until we should update graphs
		if (limitGraphUpdates && Time.time-lastGraphUpdate < maxGraphUpdateFreq) {
			if (!graphUpdateRoutineRunning) {
				StartCoroutine (DelayedGraphUpdate ());
			}
		} else {
			// Otherwise, graph updates should be carried out as soon as possible
			QueueGraphUpdates ();
		}
	}

	/** Forces graph updates to run.
	 * This will force all graph updates to run immidiately. Or more correctly, it will block the Unity main thread until graph updates can be performed and then issue them.
	 * This will force the pathfinding threads to finish calculate the path they are currently calculating (if any) and then pause.
	 * When all threads have paused, graph updates will be performed.
	 * \warning Using this very often (many times per second) can reduce your fps due to a lot of threads waiting for one another.
	 * But you probably wont have to worry about that.
	 *
	 * \note This is almost identical to FlushThreadSafeCallbacks, but added for more descriptive name.
	 * This function will also override any time limit delays for graph updates.
	 * This is because graph updates are implemented using thread safe callbacks.
	 * So calling this function will also call other thread safe callbacks (if any are waiting).
	 *
	 * Will not do anything if there are no graph updates queued (not even call other callbacks).
	 */
	public void FlushGraphUpdates () {
		if (IsAnyGraphUpdateQueued) {
			QueueGraphUpdates ();
			FlushWorkItems (true, true);
		}
	}

#endregion

	/** Make sure work items are executed.
	 *
	 * \param unblockOnComplete If true, pathfinding will be allowed to start running immediately after completing all work items.
	 * \param block If true, work items that usually take more than one frame to complete will be forced to complete during this call.
	 *              If false, then after this call there might still be work left to do.
	 *
	 * \see AddWorkItem
	 */
	public void FlushWorkItems ( bool unblockOnComplete = true, bool block = false ) {
		BlockUntilPathQueueBlocked();
		// Run tasks
		PerformBlockingActions(block, unblockOnComplete);
	}

	/** Forces thread safe callbacks to run.
	 * This will force all thread safe callbacks to run immidiately. Or rather, it will block the Unity main thread until callbacks can be called and then issue them.
	 * This will force the pathfinding threads to finish calculate the path they are currently calculating (if any) and then pause.
	 * When all threads have paused, thread safe callbacks will be called (which can be e.g graph updates).
	 *
	 * \warning Using this very often (many times per second) can reduce your fps due to a lot of threads waiting for one another.
	 * But you probably wont have to worry about that
	 *
	 * \note This is almost (note almost) identical to FlushGraphUpdates, but added for more appropriate name.
	 */
	public void FlushThreadSafeCallbacks () {
		// No callbacks? why wait?
		if (OnThreadSafeCallback != null) {
			BlockUntilPathQueueBlocked();
			PerformBlockingActions();
		}
	}

#if !PhotonImplementation && !AstarRelease
	[ContextMenu("Log Profiler")]
	public void LogProfiler () {
		AstarProfiler.PrintFastResults ();

	}

	[ContextMenu("Reset Profiler")]
	public void ResetProfiler () {
		AstarProfiler.Reset ();
	}
#endif

	/** Calculates number of threads to use.
	 * If \a count is not Automatic, simply returns \a count casted to an int.
	 * \returns An int specifying how many threads to use, 0 means a coroutine should be used for pathfinding instead of a separate thread.
	 *
	 * If \a count is set to Automatic it will return a value based on the number of processors and memory for the current system.
	 * If memory is <= 512MB or logical cores are <= 1, it will return 0. If memory is <= 1024 it will clamp threads to max 2.
	 * Otherwise it will return the number of logical cores clamped to 6.
	 *
	 * When running on WebGL this method always returns 0
	 */
	public static int CalculateThreadCount (ThreadCount count) {
#if UNITY_WEBGL
		return 0;
#else
		if (count == ThreadCount.AutomaticLowLoad || count == ThreadCount.AutomaticHighLoad) {
#if ASTARDEBUG
			Debug.Log (SystemInfo.systemMemorySize + " " + SystemInfo.processorCount + " " + SystemInfo.processorType);
#endif

			int logicalCores = Mathf.Max (1,SystemInfo.processorCount);
			int memory = SystemInfo.systemMemorySize;

			if ( memory <= 0 ) {
				Debug.LogError ("Machine reporting that is has <= 0 bytes of RAM. This is definitely not true, assuming 1 GiB");
				memory = 1024;
			}

			if (logicalCores <= 1) return 0;

			if (memory <= 512) return 0;

			if (count == ThreadCount.AutomaticHighLoad) {
				if (memory <= 1024) logicalCores = System.Math.Min (logicalCores,2);
			} else {
				//Always run at at most processorCount-1 threads (one core reserved for unity thread).
				// Many computers use hyperthreading, so dividing by two is used to remove the hyperthreading cores, pathfinding
				// doesn't scale well past the number of physical cores anyway
				logicalCores /= 2;
				logicalCores = Mathf.Max (1, logicalCores);

				if (memory <= 1024) logicalCores = System.Math.Min (logicalCores,2);

				logicalCores = System.Math.Min (logicalCores,6);
			}

			return logicalCores;
		} else {
			int val = (int)count;
			return val;
		}
#endif
	}

	/** Sets up all needed variables and scans the graphs.
	 * Calls Initialize, starts the ReturnPaths coroutine and scans all graphs.
	 * Also starts threads if using multithreading
	 * \see #OnAwakeSettings
	 */
	void Awake () {
		// Very important to set this. Ensures the singleton pattern holds
		active = this;

		if (FindObjectsOfType(typeof(AstarPath)).Length > 1) {
			Debug.LogError ("You should NOT have more than one AstarPath component in the scene at any time.\n" +
				"This can cause serious errors since the AstarPath component builds around a singleton pattern.");
		}

		// Disable GUILayout to gain some performance, it is not used in the OnGUI call
		useGUILayout = false;

		// This class uses the [ExecuteInEditMode] attribute
		// So Awake is called even when not playing
		// Don't do anything when not in play mode
		if (!Application.isPlaying) return;

		if (OnAwakeSettings != null) {
			OnAwakeSettings ();
		}

		// To make sure all graph modifiers have been enabled before scan (to avoid script execution order issues)
		GraphModifier.FindAllModifiers ();
		RelevantGraphSurface.FindAllGraphSurfaces ();

		InitializePathProcessor();
		InitializeProfiler();
		SetUpReferences ();
		InitializeAstarData();

#if !PhotonImplementation
		// Flush work items, possibly added in InitializeAstarData to load graph data
		FlushWorkItems();

		euclideanEmbedding.dirty = true;

		if (scanOnStartup && (!astarData.cacheStartup || astarData.file_cachedStartup == null)) {
			Scan ();
		}
#endif
	}

	/** Initializes the #pathProcessor field */
	void InitializePathProcessor () {
		int numThreads = CalculateThreadCount (threadCount);


		int numProcessors = Mathf.Max(numThreads, 1);
		bool multithreaded = numThreads > 0;
		pathProcessor = new PathProcessor(this, pathReturnQueue, numProcessors, multithreaded);

		pathProcessor.OnPathPreSearch += path => {
			var tmp = OnPathPreSearch;
			if (tmp != null) tmp(path);
		};

		pathProcessor.OnPathPostSearch += path => {
			LogPathResults(path);
			var tmp = OnPathPostSearch;
			if (tmp != null) tmp(path);
		};

		if (multithreaded) {
			graphUpdates.EnableMultithreading();
		}
	}

	/** Does simple error checking */
	internal void VerifyIntegrity () {
		if (active != this) {
			throw new System.Exception ("Singleton pattern broken. Make sure you only have one AstarPath object in the scene");
		}

		if (astarData == null) {
			throw new System.NullReferenceException ("AstarData is null... Astar not set up correctly?");
		}

		if (astarData.graphs == null) {
			astarData.graphs = new NavGraph[0];
		}
	}

	/** Internal method to make sure #active is set to this object and that #astarData is not null.
	 * Also calls OnEnable for the #colorSettings and initializes astarData.userConnections if it wasn't initialized before
	 *
	 * \warning This is mostly for use internally by the system.
	 */
	public void SetUpReferences () {
		active = this;
		if (astarData == null) {
			astarData = new AstarData ();
		}

		if (colorSettings == null) {
			colorSettings = new AstarColor ();
		}

		colorSettings.OnEnable ();
	}

	/** Calls AstarProfiler.InitializeFastProfile */
	void InitializeProfiler () {
		AstarProfiler.InitializeFastProfile (new string [14] {
			"Prepare", 			//0
			"Initialize",		//1
			"CalculateStep",	//2
			"Trace",			//3
			"Open",				//4
			"UpdateAllG",		//5
			"Add",				//6
			"Remove",			//7
			"PreProcessing",	//8
			"Callback",			//9
			"Overhead",			//10
			"Log",				//11
			"ReturnPaths",		//12
			"PostPathCallback"	//13
		});
	}

	/** Initializes the AstarData class.
	 * Searches for graph types, calls Awake on #astarData and on all graphs
	 *
	 * \see AstarData.FindGraphTypes
	 */
	void InitializeAstarData () {
		astarData.FindGraphTypes ();

		astarData.Awake ();

		astarData.UpdateShortcuts ();

		// Initialize all graphs by calling their Awake functions
		for (int i=0;i<astarData.graphs.Length;i++) {
			if (astarData.graphs[i] != null) astarData.graphs[i].Awake ();
		}
	}

	/** Cleans up meshes to avoid memory leaks */
	void OnDisable () {
		// Some graph editors use meshes in OnDrawGizmos
		// the better visualize the navmesh.
		// If these meshes are not destroyed when
		// scripts are reloaded, the scene is switched
		// and some other cases, the memory will leak
		// and the user will not be happy.
		// The editor registers to this callback
		// so that it can destroy the meshes
		// when neccessary.
		// It is a bit ugly to have to register
		// to a callback like this, but that was
		// the only way I found that worked well
		if (OnUnloadGizmoMeshes != null) {
			OnUnloadGizmoMeshes();
		}
	}

	/** Clears up variables and other stuff, destroys graphs.
	 * Note that when destroying an AstarPath object, all static variables such as callbacks will be cleared.
	 */
	void OnDestroy () {
		// This class uses the [ExecuteInEditMode] attribute
		// So OnDestroy is called even when not playing
		// Don't do anything when not in play mode
		if (!Application.isPlaying) return;

		if (logPathResults == PathLog.Heavy)
			Debug.Log ("+++ AstarPath Component Destroyed - Cleaning Up Pathfinding Data +++");

		if (active != this) return;

		// Block until the pathfinding threads have
		// completed their current path calculation
		BlockUntilPathQueueBlocked();

		euclideanEmbedding.dirty = false;
		FlushWorkItems (false, true);

		// Don't accept any more path calls to this AstarPath instance.
		// This will cause all eventual multithreading threads to exit
		pathProcessor.queue.TerminateReceivers();

		if (logPathResults == PathLog.Heavy)
			Debug.Log ("Processing Possible Work Items");

		// Stop the graph update thread (if it is running)
		graphUpdates.DisableMultithreading();

		// Try to join pathfinding threads
		pathProcessor.JoinThreads();

		if (logPathResults == PathLog.Heavy)
			Debug.Log ("Returning Paths");


		// Return all paths
		pathReturnQueue.ReturnPaths (false);

		if (logPathResults == PathLog.Heavy)
			Debug.Log ("Destroying Graphs");


		// Clean up graph data
		astarData.OnDestroy ();

		if (logPathResults == PathLog.Heavy)
			Debug.Log ("Cleaning up variables");

		// Clear variables up, static variables are good to clean up, otherwise the next scene might get weird data

		// Clear all callbacks
		OnDrawGizmosCallback	= null;
		OnAwakeSettings			= null;
		OnGraphPreScan			= null;
		OnGraphPostScan			= null;
		OnPathPreSearch			= null;
		OnPathPostSearch		= null;
		OnPreScan				= null;
		OnPostScan				= null;
		OnLatePostScan			= null;
		On65KOverflow			= null;
		OnGraphsUpdated			= null;
		OnThreadSafeCallback	= null;

		active = null;
	}

#region ScanMethods

	/** Floodfills starting from the specified node */
	public void FloodFill (GraphNode seed) {
		graphUpdates.FloodFill(seed);
	}

	/** Floodfills starting from 'seed' using the specified area */
	public void FloodFill (GraphNode seed, uint area) {
		graphUpdates.FloodFill(seed, area);
	}

	/** Floodfills all graphs and updates areas for every node.
	 * The different colored areas that you see in the scene view when looking at graphs
	 * are called just 'areas', this method calculates which nodes are in what areas.
	 * \see Pathfinding.Node.area
	 */
	[ContextMenu("Flood Fill Graphs")]
	public void FloodFill () {
		graphUpdates.FloodFill();
		workItems.OnFloodFill();
	}

	/** Returns a new global node index.
	 * \warning This method should not be called directly. It is used by the GraphNode constructor.
	 */
	internal int GetNewNodeIndex () {
		return pathProcessor.GetNewNodeIndex();
	}

	/** Initializes temporary path data for a node.
	 * \warning This method should not be called directly. It is used by the GraphNode constructor.
	 */
	internal void InitializeNode (GraphNode node) {
		pathProcessor.InitializeNode(node);
	}

	/** Internal method to destroy a given node.
	 * This is to be called after the node has been disconnected from the graph so that it cannot be reached from any other nodes.
	 * It should only be called during graph updates, that is when the pathfinding threads are either not running or paused.
	 *
	 * \warning This method should not be called by user code. It is used internally by the system.
	 */
	internal void DestroyNode (GraphNode node) {
		pathProcessor.DestroyNode(node);
	}

	/** Blocks until all pathfinding threads are paused and blocked.
	 * A call to pathProcessor.queue.Unblock is required to resume pathfinding calculations. However in
	 * most cases you should never unblock the path queue, instead let the pathfinding scripts do that in the next update.
	 * Unblocking the queue when other tasks (e.g graph updates) are running can interfere and cause invalid graphs.
	 *
	 * \note In most cases this should not be called from user code.
	 */
	public void BlockUntilPathQueueBlocked () {
		pathProcessor.BlockUntilPathQueueBlocked();
	}

	/** Scans all graphs.
	 * Calling this method will recalculate all graphs in the scene.
	 * This method is pretty slow (depending on graph type and graph complexity of course), so it is advisable to use
	 * smaller graph updates whenever possible.
	 * \see graph-updates
	 * \see ScanLoop
	 */
	public void Scan () {
		foreach (var p in ScanAsync ()) {
#if !NETFX_CORE && UNITY_EDITOR
			// Log progress to the console
			System.Console.WriteLine(p.description);
#endif
		}
	}

	/** Scans all graphs.
	 * \deprecated ScanLoop is now an IEnumerable<Progress>. Use foreach to iterate over the progress insead
	 *
	 * \see Scan
	 */
	[System.Obsolete("ScanLoop is now named ScanAsync and is an IEnumerable<Progress>. Use foreach to iterate over the progress insead")]
	public void ScanLoop (OnScanStatus statusCallback) {
		foreach (var p in ScanAsync()) {
			statusCallback(p);
		}
	}

	/** Scans all graphs. This is a IEnumerable, you can loop through it to get the progress
\code foreach (Progress progress in AstarPath.active.ScanLoop ()) {
	Debug.Log ("Scanning... " + progress.description + " - " + (progress.progress*100).ToString ("0") + "%");
} \endcode
	  * You can scan graphs asyncronously by yielding when you loop through the progress.
	  * Note that this does not guarantee a good framerate, but it will allow you
	  * to at least show a progress bar during scanning.
\code IEnumerator Start () {
	foreach (Progress progress in AstarPath.active.ScanLoop ()) {
		Debug.Log ("Scanning... " + progress.description + " - " + (progress.progress*100).ToString ("0") + "%");
		yield return null;
	}
} \endcode
	  *
	  * \see Scan
	  */
	public IEnumerable<Progress> ScanAsync () {

		if (graphs == null) {
			yield break;
		}

		isScanning = true;
		euclideanEmbedding.dirty = false;

		VerifyIntegrity ();

		BlockUntilPathQueueBlocked ();

		// Make sure all paths that are in the queue to be returned
		// are returned immediately
		// Some modifiers (e.g the funnel modifier) rely on
		// the nodes being valid when the path is returned
		pathReturnQueue.ReturnPaths(false);

		// Just in case a user did something odd in the path complete callback
		// which unblocked the path threads
		BlockUntilPathQueueBlocked ();

		if (!Application.isPlaying) {
			GraphModifier.FindAllModifiers ();
			RelevantGraphSurface.FindAllGraphSurfaces ();
		}

		RelevantGraphSurface.UpdateAllPositions ();

		astarData.UpdateShortcuts ();

		yield return new Progress (0.05F, "Pre processing graphs");

		if (OnPreScan != null) {
			OnPreScan (this);
		}

		GraphModifier.TriggerEvent (GraphModifier.EventType.PreScan);

		var watch = System.Diagnostics.Stopwatch.StartNew();

		// Destroy previous nodes
		for (int i=0;i<graphs.Length;i++) {
			if (graphs[i] != null) {
				graphs[i].GetNodes (node => {
					node.Destroy ();
					return true;
				});
			}
		}

		// Loop through all graphs and scan them one by one
		for (int i = 0; i < graphs.Length; i++) {
			// Skip null graphs
			if (graphs[i] == null) continue;

			// Just used for progress information
			// This graph will advance the progress bar from minp to maxp
			float minp = Mathf.Lerp (0.1F,0.8F,(float)(i)/(graphs.Length));
			float maxp = Mathf.Lerp (0.1F,0.8F,(float)(i+0.95F)/(graphs.Length));

			var progressDescriptionPrefix = "Scanning graph " + (i+1) + " of " + graphs.Length + " - ";

			foreach (var progress in ScanGraph(graphs[i])) {
				yield return new Progress(Mathf.Lerp(minp, maxp, progress.progress), progressDescriptionPrefix + progress.description);
			}
		}

		yield return new Progress (0.8F,"Post processing graphs");

		if (OnPostScan != null) {
			OnPostScan (this);
		}
		GraphModifier.TriggerEvent (GraphModifier.EventType.PostScan);

		try {
			FlushWorkItems(false, true);
		} catch (System.Exception e) {
			Debug.LogException (e);
		}

		yield return new Progress (0.9F, "Computing areas");

		FloodFill ();

		VerifyIntegrity ();

		yield return new Progress (0.95F, "Late post processing");

		// Signal that we have stopped scanning here
		// Note that no yields can happen after this point
		// since then other parts of the system can start to interfere
		isScanning = false;

		if (OnLatePostScan != null) {
			OnLatePostScan (this);
		}
		GraphModifier.TriggerEvent (GraphModifier.EventType.LatePostScan);

		euclideanEmbedding.dirty = true;
		euclideanEmbedding.RecalculatePivots ();

		//Perform any blocking actions and unblock (probably, some tasks might take a few frames)
		PerformBlockingActions(true);

		watch.Stop();
		lastScanTime = (float)watch.Elapsed.TotalSeconds;

		System.GC.Collect ();

		Log ("Scanning - Process took "+(lastScanTime*1000).ToString ("0")+" ms to complete");
	}

	IEnumerable<Progress> ScanGraph (NavGraph graph) {
		if (OnGraphPreScan != null) {
			yield return new Progress (0, "Pre processing");
			OnGraphPreScan (graph);
		}

		yield return new Progress (0, "");

		foreach (var p in graph.ScanInternal ()) {
			yield return new Progress(Mathf.Lerp (0, 0.95f, p.progress), p.description);
		}

		yield return new Progress(0.95f, "Assigning graph indices");

		// Assign the graph index to every node in the graph
		graph.GetNodes (node => {
			node.GraphIndex = (uint)graph.graphIndex;
			return true;
		});

		if (OnGraphPostScan != null) {
			yield return new Progress (0.99f, "Post processing");
			OnGraphPostScan (graph);
		}
	}

#endregion

	private static int waitForPathDepth = 0;

	/** Wait for the specified path to be calculated.
	 * Normally it takes a few frames for a path to get calculated and returned.
	 * This function will ensure that the path will be calculated when this function returns
	 * and that the callback for that path has been called.
	 *
	 * \note Do not confuse this with Pathfinding.Path.WaitForPath. This one will halt all operations until the path has been calculated
	 * while Pathfinding.Path.WaitForPath will wait using yield until it has been calculated.
	 *
	 * If requesting a lot of paths in one go and waiting for the last one to complete,
	 * it will calculate most of the paths in the queue (only most if using multithreading, all if not using multithreading).
	 *
	 * Use this function only if you really need to.
	 * There is a point to spreading path calculations out over several frames.
	 * It smoothes out the framerate and makes sure requesting a large
	 * number of paths at the same time does not cause lag.
	 *
	 * \note Graph updates and other callbacks might get called during the execution of this function.
	 *
	 * When the pathfinder is shutting down. I.e in OnDestroy, this function will not do anything.
	 *
	 * \param p The path to wait for. The path must be started, otherwise an exception will be thrown.
	 *
	 * \throws Exception if pathfinding is not initialized properly for this scene (most likely no AstarPath object exists)
	 * or if the path has not been started yet.
	 * Also throws an exception if critical errors occur such as when the pathfinding threads have crashed (which should not happen in normal cases).
	 * This prevents an infinite loop while waiting for the path.
	 *
	 * \see Pathfinding.Path.WaitForPath
	 */
	public static void WaitForPath (Path p) {

		if (active == null)
			throw new System.Exception ("Pathfinding is not correctly initialized in this scene (yet?). " +
				"AstarPath.active is null.\nDo not call this function in Awake");

		if (p == null) throw new System.ArgumentNullException ("Path must not be null");

		if (active.pathProcessor.queue.IsTerminating) return;

		if (p.GetState () == PathState.Created){
			throw new System.Exception ("The specified path has not been started yet.");
		}

		waitForPathDepth++;

		if (waitForPathDepth == 5) {
			Debug.LogError ("You are calling the WaitForPath function recursively (maybe from a path callback). Please don't do this.");
		}

		if (p.GetState() < PathState.ReturnQueue) {
			if (active.IsUsingMultithreading) {

				while (p.GetState() < PathState.ReturnQueue) {
					if (active.pathProcessor.queue.IsTerminating) {
						waitForPathDepth--;
						throw new System.Exception ("Pathfinding Threads seems to have crashed.");
					}

					//Wait for threads to calculate paths
					Thread.Sleep(1);
					active.PerformBlockingActions();
				}
			} else {
				while (p.GetState() < PathState.ReturnQueue) {
					if (active.pathProcessor.queue.IsEmpty && p.GetState () != PathState.Processing) {
						waitForPathDepth--;
						throw new System.Exception ("Critical error. Path Queue is empty but the path state is '" + p.GetState() + "'");
					}

					//Calculate some paths
					active.pathProcessor.TickNonMultithreaded();
					active.PerformBlockingActions();
				}
			}
		}

		active.pathReturnQueue.ReturnPaths (false);

		waitForPathDepth--;
	}

	/** Will send a callback when it is safe to update nodes. This is defined as between the path searches.
	  * This callback will only be sent once and is nulled directly after the callback has been sent.
	  * When using more threads than one, calling this often might decrease pathfinding performance due to a lot of idling in the threads.
	  * Not performance as in it will use much CPU power,
	  * but performance as in the number of paths per second will probably go down (though your framerate might actually increase a tiny bit)
	  *
	  * You should only call this function from the main unity thread (i.e normal game code).
	  *
	  * \note The threadSafe parameter has been deprecated
	  * \deprecated
	  */
	[System.Obsolete ("The threadSafe parameter has been deprecated")]
	public static void RegisterSafeUpdate (System.Action callback, bool threadSafe) {
		RegisterSafeUpdate ( callback );
	}

	/** Will send a callback when it is safe to update nodes. This is defined as between the path searches.
	  * This callback will only be sent once and is nulled directly after the callback has been sent.
	  * When using more threads than one, calling this often might decrease pathfinding performance due to a lot of idling in the threads.
	  * Not performance as in it will use much CPU power,
	  * but performance as in the number of paths per second will probably go down (though your framerate might actually increase a tiny bit)
	  *
	  * You should only call this function from the main unity thread (i.e normal game code).
	  *
	  * \code
Node node = AstarPath.active.GetNearest (transform.position).node;
AstarPath.RegisterSafeUpdate (delegate () {
	node.walkable = false;
});
\endcode

\code
Node node = AstarPath.active.GetNearest (transform.position).node;
AstarPath.RegisterSafeUpdate (delegate () {
	node.position = (Int3)transform.position;
});
\endcode
	  *
	  *
	  */
	public static void RegisterSafeUpdate (System.Action callback) {
		if (callback == null || !Application.isPlaying) {
			return;
		}

		// If all pathfinding threads are already blocked
		// we might as well just call the callback immediately
		if (active.pathProcessor.queue.AllReceiversBlocked) {
			// We need to lock here since we cannot be sure that this is the main
			// Unity thread and therefore we cannot be sure that some other thread
			// will not try to unblock the queue while we are processing the callback
			active.pathProcessor.queue.Lock();
			try {
				// Check again as another thread might have unblocked the queue
				if (active.pathProcessor.queue.AllReceiversBlocked) {
					callback ();
					return;
				}
				// If that check failed, it will fall back to the code below
			} finally {
				active.pathProcessor.queue.Unlock();
			}
		}

		// Lock while modifying the callback
		lock (safeUpdateLock) {
			// OnSafeCallback has been deprecated
			OnThreadSafeCallback += callback;
		}

		// Block path queue so that the above callbacks may be called
		active.pathProcessor.queue.Block();
	}

	/** Blocks the path queue so that e.g work items can be performed */
	void InterruptPathfinding () {
		pathProcessor.queue.Block();
	}

	/** Puts the Path in queue for calculation.
	  * The callback specified when constructing the path will be called when the path has been calculated.
	  * Usually you should use the Seeker component instead of calling this function directly.
	  *
	  * \param p The path that should be put in queue for calculation
	  * \param pushToFront If true, the path will be pushed to the front of the queue, bypassing all waiting paths and making it the next path to be calculated.
	  * This can be useful if you have a path which you want to prioritize over all others. Be careful to not overuse it though.
	  * If too many paths are put in the front of the queue often, this can lead to normal paths having to wait a very long time before being calculated.
	  */
	public static void StartPath (Path p, bool pushToFront = false) {
		// Copy to local variable to avoid multithreading issues
		var astar = active;

		if (System.Object.ReferenceEquals (astar, null)) {
			Debug.LogError ("There is no AstarPath object in the scene or it has not been initialized yet");
			return;
		}

		if (p.GetState() != PathState.Created) {
			throw new System.Exception ("The path has an invalid state. Expected " + PathState.Created + " found " + p.GetState() + "\n" +
				"Make sure you are not requesting the same path twice");
		}

		if (astar.pathProcessor.queue.IsTerminating) {
			p.Error ();
			p.LogError ("No new paths are accepted");
			return;
		}

		if (astar.graphs == null || astar.graphs.Length == 0) {
			Debug.LogError ("There are no graphs in the scene");
			p.Error ();
			p.LogError ("There are no graphs in the scene");
			Debug.LogError (p.errorLog);
			return;
		}

		p.Claim (astar);

		// Will increment p.state to PathState.PathQueue
		p.AdvanceState (PathState.PathQueue);
		if (pushToFront) {
			astar.pathProcessor.queue.PushFront (p);
		} else {
			astar.pathProcessor.queue.Push (p);
		}
	}

#if PhotonImplementation
	/** Replacement for UnityEngine's StartCoroutine */
	public void StartCoroutine (IEnumerator update) {
		if (activeThread != null && activeThread.IsAlive) {
			Debug.Log ("Can only start one coroutine at a time, please end the current running thread first (activeThread)");
			return;
		}

		//Execute to the first yield
		if (!update.MoveNext ()) {
			return;
		}

		activeThread = new Thread (StartCoroutineInternal);
		activeThread.Start (update);
	}

	/** Replacement for UnityEngine's StartCoroutine */
	private void StartCoroutineInternal (System.Object updateOb) {

		IEnumerator update = (IEnumerator)updateOb;

		if (update == null) {
			return;
		}

		while (update.MoveNext ()) {
			Thread.Sleep(1);
		}
	}
#endif

	/** Terminates pathfinding threads when the application quits */
	void OnApplicationQuit () {
		OnDestroy ();

		// Abort threads if they are still running (likely because of some bug in that case)
		// to make sure that the application can shut down properly
		pathProcessor.AbortThreads();
	}

	/**
	 * Returns the nearest node to a position using the specified NNConstraint.
	 * Searches through all graphs for their nearest nodes to the specified position and picks the closest one.\n
	 * Using the NNConstraint.None constraint.
	 * \see Pathfinding.NNConstraint
	 */
	public NNInfo GetNearest (Vector3 position) {
		return GetNearest(position,NNConstraint.None);
	}

	/**
	 * Returns the nearest node to a position using the specified NNConstraint.
	 * Searches through all graphs for their nearest nodes to the specified position and picks the closest one.
	 * The NNConstraint can be used to specify constraints on which nodes can be chosen such as only picking walkable nodes.
	 * \see Pathfinding.NNConstraint
	 */
	public NNInfo GetNearest (Vector3 position, NNConstraint constraint) {
		return GetNearest(position,constraint,null);
	}

	/**
	 * Returns the nearest node to a position using the specified NNConstraint.
	 * Searches through all graphs for their nearest nodes to the specified position and picks the closest one.
	 * The NNConstraint can be used to specify constraints on which nodes can be chosen such as only picking walkable nodes.
	 * \see Pathfinding.NNConstraint
	 */
	public NNInfo GetNearest (Vector3 position, NNConstraint constraint, GraphNode hint) {

		if (graphs == null) return new NNInfo();

		float minDist = float.PositiveInfinity;
		NNInfo nearestNode = new NNInfo ();
		int nearestGraph = -1;

		for (int i = 0; i < graphs.Length; i++) {
			NavGraph graph = graphs[i];

			// Check if this graph should be searched
			if (graph == null || !constraint.SuitableGraph (i,graph)) {
				continue;
			}

			NNInfo nnInfo;
			if (fullGetNearestSearch) {
				nnInfo = graph.GetNearestForce (position, constraint);
			} else {
				nnInfo = graph.GetNearest (position, constraint);
			}

			GraphNode node = nnInfo.node;

			if (node == null) {
				continue;
			}

			float dist = ((Vector3)nnInfo.clampedPosition-position).magnitude;

			if (prioritizeGraphs && dist < prioritizeGraphsLimit) {
				// The node is close enough, choose this graph and discard all others
				minDist = dist;
				nearestNode = nnInfo;
				nearestGraph = i;
				break;
			} else {
				if (dist < minDist) {
					minDist = dist;
					nearestNode = nnInfo;
					nearestGraph = i;
				}
			}
		}

		// No matches found
		if (nearestGraph == -1) {
			return nearestNode;
		}

		// Check if a constrained node has already been set
		if (nearestNode.constrainedNode != null) {
			nearestNode.node = nearestNode.constrainedNode;
			nearestNode.clampedPosition = nearestNode.constClampedPosition;
		}

		if (!fullGetNearestSearch && nearestNode.node != null && !constraint.Suitable (nearestNode.node)) {
			// Otherwise, perform a check to force the graphs to check for a suitable node
			NNInfo nnInfo = graphs[nearestGraph].GetNearestForce (position, constraint);

			if (nnInfo.node != null) {
				nearestNode = nnInfo;
			}
		}

		if (!constraint.Suitable (nearestNode.node) || (constraint.constrainDistance && (nearestNode.clampedPosition - position).sqrMagnitude > maxNearestNodeDistanceSqr)) {
			return new NNInfo();
		}

		return nearestNode;
	}

#if !PhotonImplementation
	/** Returns the node closest to the ray (slow).
	 * \warning This function is brute-force and very slow, it can barely be used once per frame
	 */
	public GraphNode GetNearest (Ray ray) {

		if (graphs == null) return null;

		float minDist = Mathf.Infinity;
		GraphNode nearestNode = null;

		Vector3 lineDirection = ray.direction;
		Vector3 lineOrigin = ray.origin;

		for (int i=0;i<graphs.Length;i++) {

			NavGraph graph = graphs[i];

			graph.GetNodes (node => {
	        	Vector3 pos = (Vector3)node.position;
				Vector3 p = lineOrigin+(Vector3.Dot(pos-lineOrigin,lineDirection)*lineDirection);

				float tmp = Mathf.Abs (p.x-pos.x);
				tmp *= tmp;
				if (tmp > minDist) return true;

				tmp = Mathf.Abs (p.z-pos.z);
				tmp *= tmp;
				if (tmp > minDist) return true;

				float dist = (p-pos).sqrMagnitude;

				if (dist < minDist) {
					minDist = dist;
					nearestNode = node;
				}
				return true;
			});

		}

		return nearestNode;
	}
#endif
}
