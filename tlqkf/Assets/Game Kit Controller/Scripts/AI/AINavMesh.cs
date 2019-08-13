using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class AINavMesh : MonoBehaviour
{
	public NavMeshAgent agent;
	public playerController playerControllerManager;
	public playerCamera playerCameraManager;

	public Transform rayCastPosition;

	public float targetChangeTolerance = 1;

	public float minDistanceToFriend;
	public float minDistanceToEnemy;
	public float minDistanceToObjects;

	public float minDistanceToMoveBack;

	public float patrolSpeed;
	public bool runFromTarget;

	public float maxDistanceToRecalculatePath = 1;

	public bool showPathRenderer = true;
	public bool smoothSpeedWhenCloseToTarget = true;

	public float updateNavMeshPositionRate = 0.5f;
	public float calculatePathRate = 0.5f;

	public UnityEvent eventToPauseAI;
	public UnityEvent eventToResumeAI;

	public bool targetIsFriendly;
	public bool targetIsObject;

	public bool navMeshPaused;
	public bool patrolling;
	public bool patrollingPaused;
	public UnityEvent eventOnPartnerFound;

	public UnityEvent eventOnDeath;

	public string addAIToFriendListManagerEventName = "Add AI To Friend List Manager";
	public string removeAIToFriendListManagerEventName = "Remove AI From Friend List Manager";

	public bool useDynamicObstacleDetection;
	public LayerMask dynamicObstacleLayer;
	public float dynamicAvoidSpeed = 4;

	public Transform currentTarget;

	public bool attacking;
	public bool following;
	public bool waiting;
	public bool hiding;

	public bool followingTarget;

	public bool usingSamplePosition;

	public bool checkingToResumeNavmesh;

	public bool onGround;

	public bool showGizmo;

	public float navSpeed = 1;
	Vector3 currentTargetPosition;
	Color c = Color.white;

	OffMeshLinkData _currLink;
	bool lookingPathAfterJump;

	AINavMeshMoveInfo AIMoveInput = new AINavMeshMoveInfo ();
	LineRenderer lineRenderer;
	Transform partner;
	Vector3 targetOffset;
	float currentMinDistance;

	float originalMinDistanceToFriend;
	float originalMinDistanceToEnemy;

	Vector3 lastTargetPosition;
	NavMeshPath currentNavMeshPath;
	List<Vector3> pathCorners = new List<Vector3> ();
	Vector3 lastPathTargetPosition;
	NavMeshHit hit;
	Vector3 currentDestinationPosition;
	Vector3 samplePosition;
	float positionOffset = 0.1f;
	Coroutine waitToResumeCoroutine;
	float currentDistanceToTarget;
	float lastTimeCalculatePath;
	float lastTimeUpdateNavMeshPosition;
	Vector3 currentPosition;
	Vector3 targetCurrentPosition;

	remoteEventSystem partnerRemoteEventSystem;

	RaycastHit rayhit;

	bool obstacelDetected;
	bool moveToRight = true;

	Vector3 avoidDirection;

	Vector3 currentCheckObstalceDirection;

	Vector3 newMoveDirection;
	Vector3 currentMoveDirection;

	void Start ()
	{
		if (showPathRenderer && !GetComponent<LineRenderer> ()) {
			gameObject.AddComponent<LineRenderer> ();
			lineRenderer = GetComponent<LineRenderer> ();
			lineRenderer.material = new Material (Shader.Find ("Sprites/Default")) { color = c };
			lineRenderer.startWidth = 0.5f;
			lineRenderer.endWidth = 0.5f;
			lineRenderer.startColor = c;
			lineRenderer.endColor = c;
		}
		originalMinDistanceToFriend = minDistanceToFriend;
		originalMinDistanceToEnemy = minDistanceToEnemy;
	}

	public void setAgentDestination (Vector3 targetPosition)
	{
		if (GKC_Utils.distance (lastTargetPosition, targetPosition) > maxDistanceToRecalculatePath || Time.time > lastTimeUpdateNavMeshPosition + updateNavMeshPositionRate) {
			lastTargetPosition = targetPosition;
			agent.SetDestination (targetPosition);
			lastTimeUpdateNavMeshPosition = Time.time;
		}
		agent.transform.position = transform.position;
	}

	public bool updateCurrentNavMeshPath (Vector3 targetPosition)
	{
		bool hasFoundPath = true;
		if (GKC_Utils.distance (lastPathTargetPosition, targetPosition) > maxDistanceToRecalculatePath || pathCorners.Count == 0 || Time.time > calculatePathRate + lastTimeCalculatePath) {
			lastPathTargetPosition = targetPosition;
			setAgentDestination (lastPathTargetPosition);
			currentNavMeshPath = new NavMeshPath ();
			pathCorners.Clear ();
			hasFoundPath = agent.CalculatePath (lastPathTargetPosition, currentNavMeshPath);
			pathCorners.AddRange (currentNavMeshPath.corners);
			lastTimeCalculatePath = Time.time;
		}
		return hasFoundPath;
	}

	public void setOnGroundState (bool state)
	{
		onGround = state;
	}

	void Update ()
	{
		if (!navMeshPaused) {

			if (onGround) {
				if (lookingPathAfterJump) {
					lookingPathAfterJump = false;
				}
			}
			if (checkingToResumeNavmesh) {
				if (onGround) {
					agent.isStopped = false;
					checkingToResumeNavmesh = false;
					usingSamplePosition = false;
					return;
				} else {
					return;
				}
			}

			if (currentTarget && agent.enabled) {
				currentPosition = transform.position;
				targetCurrentPosition = currentTarget.position;
				currentDistanceToTarget = GKC_Utils.distance (targetCurrentPosition, currentPosition);
				if (patrolling) {
					currentTargetPosition = targetCurrentPosition + currentTarget.up * positionOffset;
					followingTarget = true;
					if (patrollingPaused) {
						navSpeed = 0;
					} else {
						navSpeed = patrolSpeed;
					}
				} else {
					if (runFromTarget) {
						Vector3 direction = currentPosition - targetCurrentPosition;
						targetOffset = direction / currentDistanceToTarget;
						currentTargetPosition = targetCurrentPosition + currentTarget.up * positionOffset + targetOffset * currentDistanceToTarget * 10;

						// use the values to move the character
						if (smoothSpeedWhenCloseToTarget) {
							navSpeed = 20 / currentDistanceToTarget;
							navSpeed = Mathf.Clamp (navSpeed, 0.1f, 1);
						} else {
							navSpeed = 1;
						}
						followingTarget = true;
					} else {
						if (targetIsObject) {
							currentMinDistance = minDistanceToObjects;
						} else {
							if (targetIsFriendly) {
								currentMinDistance = minDistanceToFriend;
							} else {
								currentMinDistance = minDistanceToEnemy;
							}
						}

						if (currentDistanceToTarget > currentMinDistance) {
							// update the progress if the character has made it to the previous target
							//if ((target.position - targetPos).magnitude > targetChangeTolerance) {
							currentTargetPosition = targetCurrentPosition + currentTarget.up * positionOffset;
							// use the values to move the character
							if (smoothSpeedWhenCloseToTarget) {
								navSpeed = currentDistanceToTarget / 20;
								navSpeed = Mathf.Clamp (navSpeed, 0.1f, 1);
							} else {
								navSpeed = 1;
							}
							followingTarget = true;
						} else if (currentDistanceToTarget < minDistanceToMoveBack) {
							// update the progress if the character has made it to the previous target
							Vector3 direction = currentPosition - targetCurrentPosition;
							targetOffset = direction / currentDistanceToTarget;
							currentTargetPosition = targetCurrentPosition + currentTarget.up * positionOffset + targetOffset * (minDistanceToMoveBack + 1);
							// use the values to move the character
							navSpeed = currentDistanceToTarget / 20;
							navSpeed = Mathf.Clamp (navSpeed, 0.1f, 1);
							followingTarget = true;
						} else {
							// We still need to call the character's move function, but we send zeroed input as the move param.
							moveNavMesh (Vector3.zero, false, false);
							followingTarget = false;
							usingSamplePosition = false;

							if (targetIsObject) {
								removeTarget ();
							}
						}
					}
				}

				if (followingTarget) {
					if ((!targetIsFriendly || targetIsObject) && !patrolling) {
						navSpeed = 1;
					}

					if (!usingSamplePosition && !lookingPathAfterJump) {
						setAgentDestination (currentTargetPosition);
						currentDestinationPosition = currentTargetPosition;
					}
				
					if (!updateCurrentNavMeshPath (currentDestinationPosition)) {
						//print ("first path search not complete");

						bool getClosestPosition = false;

						if (currentNavMeshPath.status != NavMeshPathStatus.PathComplete) {
							getClosestPosition = true;
						}

						if (getClosestPosition) {
							//get closest position to target that can be reached
							//maybe a for bucle checking every position ofthe corner and get the latest reachable
							//float currentTargetPositionDistace = Vector3.Distance (currentDestinationPosition, transform.position);
							Vector3 positionToGet = currentDestinationPosition;
							if (pathCorners.Count > 1) {
								positionToGet = pathCorners [pathCorners.Count - 2];
							}

							usingSamplePosition = true;
						
							if (NavMesh.SamplePosition (positionToGet + Vector3.up, out hit, 4, agent.areaMask)) {
								if (samplePosition != hit.position) {
									samplePosition = hit.position;
									currentDestinationPosition = hit.position;
									updateCurrentNavMeshPath (hit.position);
								}
							}
						} else {
							usingSamplePosition = false;
						}
					}

					if (usingSamplePosition) {
						agent.SetDestination (lastPathTargetPosition);
						agent.transform.position = currentPosition;
						if (agent.remainingDistance < 0.1f || GKC_Utils.distance (lastPathTargetPosition, currentTargetPosition) > maxDistanceToRecalculatePath) {
							usingSamplePosition = false;
						}
					}

					if (currentNavMeshPath != null) {
						if (currentNavMeshPath.status == NavMeshPathStatus.PathComplete) {
							//print ("Can reach" +agent.desiredVelocity);
							moveNavMesh (agent.desiredVelocity * navSpeed, false, false);
							c = Color.white;
						} else if (currentNavMeshPath.status == NavMeshPathStatus.PathPartial) {
							c = Color.yellow;
							if (GKC_Utils.distance (currentPosition, pathCorners [pathCorners.Count - 1]) > 2) {
								moveNavMesh (agent.desiredVelocity * navSpeed, false, false);
							} else {
								moveNavMesh (Vector3.zero, false, false);
							}
							//print ("Can get close");
						} else if (currentNavMeshPath.status == NavMeshPathStatus.PathInvalid) {
							c = Color.red;
							//print ("Can't reach");
						}
				
						if (agent.isOnOffMeshLink && !lookingPathAfterJump) {
							_currLink = agent.currentOffMeshLinkData;
							lookingPathAfterJump = true;
							moveNavMesh (agent.desiredVelocity * navSpeed, false, true);
						} 
					}

					if (showPathRenderer) {
						lineRenderer.enabled = true;
						lineRenderer.startColor = c;
						lineRenderer.endColor = c;
						lineRenderer.positionCount = pathCorners.Count;
						for (int i = 0; i < pathCorners.Count; i++) {
							lineRenderer.SetPosition (i, pathCorners [i]);
						}
					}
				} else {
					if (showPathRenderer) {
						lineRenderer.enabled = false;
					}
				}
			} else {
				moveNavMesh (Vector3.zero, false, false);
				usingSamplePosition = false;
				if (followingTarget) {
					if (agent.enabled) {
						agent.ResetPath ();
					}
					followingTarget = false;
				}
				if (showPathRenderer) {
					lineRenderer.enabled = false;
				}
			}
				
		} else {
			usingSamplePosition = false;
			if (followingTarget) {
				if (agent.enabled) {
					agent.ResetPath ();
				}
				followingTarget = false;
			}
			if (showPathRenderer) {
				lineRenderer.enabled = false;
			}
		}
	}

	public Vector3 getMovementDirection ()
	{
		if (pathCorners.Count > 0) {
			if (GKC_Utils.distance (pathCorners [0], transform.position) < 0.1f) {
				pathCorners.RemoveAt (0);
			}
			if (pathCorners.Count > 0) {
				Vector3 direction = pathCorners [0] - transform.position;
				return direction.normalized;
			}
		}
		return Vector3.zero;
	}

	public void moveNavMesh (Vector3 move, bool crouch, bool jump)
	{
		if (useDynamicObstacleDetection) {
			if (move != Vector3.zero) {
				Vector3 raycastPosition = transform.position + transform.up;

				if (!obstacelDetected) {
					if (Physics.Raycast (raycastPosition, transform.forward, out rayhit, 3, dynamicObstacleLayer)) {
						obstacelDetected = true;
						avoidDirection = move.normalized;

						currentCheckObstalceDirection = Quaternion.Euler (transform.up * 60) * transform.forward;
		
						if (Physics.Raycast (raycastPosition, currentCheckObstalceDirection, out rayhit, 3, dynamicObstacleLayer)) {

							moveToRight = false;
						}
					} else {
						currentCheckObstalceDirection = Quaternion.Euler (transform.up * 60) * transform.forward;
						if (Physics.Raycast (raycastPosition, currentCheckObstalceDirection, out rayhit, 0.5f, dynamicObstacleLayer)) {
						
							obstacelDetected = true;
							avoidDirection = move.normalized;

							moveToRight = false;
						} else {
							currentCheckObstalceDirection = Quaternion.Euler (-transform.up * 60) * transform.forward;
							if (Physics.Raycast (raycastPosition, currentCheckObstalceDirection, out rayhit, 0.5f, dynamicObstacleLayer)) {

								obstacelDetected = true;
								avoidDirection = move.normalized;
							} else {
								currentCheckObstalceDirection = Quaternion.Euler (transform.up * 90) * transform.forward;
								if (Physics.Raycast (raycastPosition, currentCheckObstalceDirection, out rayhit, 1, dynamicObstacleLayer)) {

									obstacelDetected = true;
									avoidDirection = move.normalized;

									moveToRight = false;
								} else {
									currentCheckObstalceDirection = Quaternion.Euler (-transform.up * 90) * transform.forward;
									if (Physics.Raycast (raycastPosition, currentCheckObstalceDirection, out rayhit, 1, dynamicObstacleLayer)) {

										obstacelDetected = true;
										avoidDirection = move.normalized;
									}
								}
							}
						}
					}
				}

				if (obstacelDetected) {

					float obstacleDistance = 3;

					bool newObstacleFound = false;
					Vector3 rayDirection = Vector3.zero;
					if (Physics.Raycast (raycastPosition, transform.forward, out rayhit, 3, dynamicObstacleLayer)) {
					
						obstacleDistance = rayhit.distance;

						newObstacleFound = true;
					}
					//Debug.DrawRay (raycastPosition, transform.forward, Color.green, 2);
					
					rayDirection = Quaternion.Euler (transform.up * 45) * transform.forward;
					if (Physics.Raycast (raycastPosition, rayDirection, out rayhit, 4, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}
					}
					//Debug.DrawRay (raycastPosition rayDirection, Color.green, 2);

					rayDirection = Quaternion.Euler (-transform.up * 45) * transform.forward;
					if (Physics.Raycast (raycastPosition, rayDirection, out rayhit, 4, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}
					}
					//Debug.DrawRay (raycastPosition, rayDirection, Color.green, 2);

					rayDirection = Quaternion.Euler (transform.up * 90) * transform.forward;
					if (Physics.Raycast (raycastPosition, rayDirection, out rayhit, 4, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}
					}
					//Debug.DrawRay (raycastPosition, rayDirection, Color.green, 2);

					rayDirection = Quaternion.Euler (-transform.up * 90) * transform.forward;
					if (Physics.Raycast (raycastPosition, rayDirection, out rayhit, 4, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}
					}
					//Debug.DrawRay (raycastPosition, rayDirection, Color.green, 2);

					if (!newObstacleFound) {
						obstacelDetected = false;
						moveToRight = true;
					} else {
						float avoidAngle = 30;
						if (obstacleDistance < 3) {
							avoidAngle = 45;
						} 

						if (obstacleDistance < 2) {
							avoidAngle = 65;
						} 

						if (obstacleDistance < 1) {
							avoidAngle = 85;
						}

						if (!moveToRight) {
							avoidAngle = -avoidAngle;
						}

						Vector3 newDirection = Quaternion.Euler (transform.up * avoidAngle) * avoidDirection;
						newDirection = newDirection.normalized;
						newMoveDirection = newDirection;
					}
				} else {
					newMoveDirection = move;
				}
			}else {
				newMoveDirection = move;
			}

			currentMoveDirection = Vector3.Lerp (currentMoveDirection, newMoveDirection, Time.deltaTime * dynamicAvoidSpeed);

			move = currentMoveDirection;
		}

		AIMoveInput.moveInput = move;
		AIMoveInput.crouchInput = crouch;
		AIMoveInput.jumpInput = jump;

		playerControllerManager.Move (AIMoveInput);
		playerCameraManager.Rotate (rayCastPosition.forward);
	}

	public void pauseAI (bool state)
	{
		navMeshPaused = state;
		if (navMeshPaused) {
			if (agent) {
				if (agent.enabled) {
					agent.isStopped = true;
					agent.enabled = false;
				}
			}
		} else {
			if (!agent.enabled) {
				agent.enabled = true;
				if (waitToResumeCoroutine != null) {
					StopCoroutine (waitToResumeCoroutine);
				}
				waitToResumeCoroutine = StartCoroutine (resumeCoroutine ());
			}
		}

		if (navMeshPaused) {
			eventToPauseAI.Invoke ();
		} else {
			eventToResumeAI.Invoke ();
		}

		if (showPathRenderer) {
			lineRenderer.enabled = !state;
		}
	}

	IEnumerator resumeCoroutine ()
	{
		yield return new WaitForSeconds (0.0001f);
		if (agent.enabled) {
			agent.isStopped = false;
		}
		checkingToResumeNavmesh = true;
	}

	//disable ai when it dies
	public void setAIStateToDead ()
	{
		eventOnDeath.Invoke ();
	}

	public void recalculatePath ()
	{
		if (agent.enabled) {
			agent.isStopped = false;
		}
	}

	public void jumpStart ()
	{
		
	}

	public void jumpEnded ()
	{
		agent.enabled = true;
		agent.CompleteOffMeshLink ();
		//Resume normal navmesh behaviour
		agent.isStopped = false;
		lookingPathAfterJump = false;
		navMeshPaused = false;
	}

	public void setTarget (Transform newTarget)
	{
		currentTarget = newTarget;
	}

	public void avoidTarget (Transform targetToAvoid)
	{
		currentTarget = targetToAvoid;
	}

	public void setAvoidTargetState (bool state)
	{
		runFromTarget = state;
	}

	//	public void setTargetOffset(Vector3 offset){
	//		targetOffset = offset;
	//	}

	public void removeTarget ()
	{
		currentTarget = null;
	}

	public void partnerFound (Transform currentPartner)
	{
		partner = currentPartner;
		currentTarget = partner;

		if (patrolling) {
			patrolling = false;
		}

		eventOnPartnerFound.Invoke ();

		partnerRemoteEventSystem = currentTarget.GetComponent<remoteEventSystem> ();
		if (partnerRemoteEventSystem) {
			partnerRemoteEventSystem.callRemoveEventWithGameObject (addAIToFriendListManagerEventName, gameObject);
		}
	}

	public void removeFromPartnerList ()
	{
		if (partner) {
			if (partnerRemoteEventSystem) {
				partnerRemoteEventSystem.callRemoveEventWithTransform (removeAIToFriendListManagerEventName, transform);
			}
		}
	}

	public void removePartner ()
	{
		if (partner) {
			removeFromPartnerList ();

			if (currentTarget == partner) {
				currentTarget = null;
			}

			partner = null;
		}
	}

	public void lookAtTaget (bool state)
	{
		//make the character to look or not to look towars its target, pointing the camera towards it
		AIMoveInput.lookAtTarget = state;
	}

	public void setPatrolState (bool state)
	{
		patrolling = state;
	}

	public void setPatrolPauseState (bool state)
	{
		patrollingPaused = state;
	}

	public void setPatrolSpeed (float value)
	{
		patrolSpeed = value;
	}

	public void attack (Transform newTarget)
	{
		setTarget (newTarget);
		setCharacterStates (true, false, false, false);
	}

	public void follow (Transform newTarget)
	{
		followingTarget = true;
		setTarget (newTarget);
		setCharacterStates (false, true, false, false);
	}

	public void wait (Transform newTarget)
	{
		removeTarget ();
		setCharacterStates (false, false, true, false);
	}

	public void hide (Transform newTarget)
	{
		setTarget (newTarget);
		setCharacterStates (false, false, false, true);
	}

	public bool isCharacterAttacking ()
	{
		return attacking;
	}

	public bool isCharacterFollowing ()
	{
		return following;
	}

	public bool isCharacterWaiting ()
	{
		return waiting;
	}

	public bool isCharacterHiding ()
	{
		return hiding;
	}

	public void setCharacterStates (bool attackValue, bool followValue, bool waitValue, bool hideValue)
	{
		attacking = attackValue;
		following = followValue;
		waiting = waitValue;
		hiding = hideValue;
	}

	public void setTargetType (bool isFriendly, bool isObject)
	{
		targetIsFriendly = isFriendly;
		targetIsObject = isObject;
		if (targetIsFriendly) {
			setCharacterStates (false, false, false, false);
		}
	}

	public void setExtraMinDistanceState (bool state, float extraDistance)
	{
		if (state) {
			minDistanceToFriend = originalMinDistanceToFriend + extraDistance;
			minDistanceToEnemy = originalMinDistanceToEnemy + extraDistance;
		} else {
			minDistanceToFriend = originalMinDistanceToFriend;
			minDistanceToEnemy = originalMinDistanceToEnemy;
		}
	}

	public bool isFollowingTarget ()
	{
		return followingTarget;
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		pauseAI (state);
	}

	void OnDrawGizmos ()
	{
		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (lastPathTargetPosition, 1);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere (samplePosition, 1);
		}
	}
}