using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class checkCollisionType : MonoBehaviour
{
	public bool onCollisionEnter;
	public bool onCollisionExit;
	public bool onTriggerEnter;
	public bool onTriggerExit;
	public bool onTriggerStay;
	public string onCollisionEnterFunctionName;
	public string onCollisionExitFunctionName;
	public string onTriggerEnterFunctionName;
	public string onTriggerExitFunctionName;
	public string onTriggerStayFunctionName;
	public GameObject parent;
	public bool active;
	public GameObject objectToCollide;

	public bool useEvents;
	public UnityEvent onCollisionEnterEvent = new UnityEvent ();
	public UnityEvent onCollisionExitEvent = new UnityEvent ();
	public UnityEvent onTriggerEnterEvent = new UnityEvent ();
	public UnityEvent onTriggerExitEvent = new UnityEvent ();
	public UnityEvent onTriggerStayEvent = new UnityEvent ();
	//a script to check all the type of collisions of an object, and in that case, send a message to another object according to the type of collision
	//if you want to use a collision enter, check the bool onCollisionEnter in the editor, set the funcion called in the onCollisionEnterFunctionName string
	//and finally set the parent, the object which will receive the function
	//also, you can set an specific object to check a collision with that object
	//the variable active can be used to check when the collision happens

//	void Start(){
//		print (gameObject.name);
//	}

	void OnCollisionEnter (Collision col)
	{
		checkOnCollision (col, true);
	}

	void OnCollisionExit (Collision col)
	{
		checkOnCollision (col, false);
	}

	public void checkOnCollision (Collision col, bool isEnter)
	{
		if (isEnter) {
			if (onCollisionEnter) {
				if (objectToCollide) {
					if (col.gameObject == objectToCollide) {
						if (onCollisionEnterFunctionName != "" && parent) {
							if (useEvents) {
								callEvent (onCollisionEnterEvent);
							} else {
								parent.SendMessage (onCollisionEnterFunctionName, col.gameObject);
							}
							active = true;
						} else {
							active = true;
						}
					}
				} else {
					if (useEvents) {
						callEvent (onCollisionEnterEvent);
					} else {
						if (onCollisionEnterFunctionName != "" && parent) {
							parent.SendMessage (onCollisionEnterFunctionName, col.gameObject);
						}
						active = true;
					}
				}
			}
		} else {
			if (onCollisionExit) {
				active = true;
				if (useEvents) {
					callEvent (onCollisionExitEvent);
				} else {
					if (onCollisionExitFunctionName != "" && parent) {
						parent.SendMessage (onCollisionExitFunctionName, col.gameObject);
					}
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		checkTrigger (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTrigger (col, false);
	}

	public void checkTrigger (Collider col, bool isEnter)
	{
		if (isEnter) {
			if (onTriggerEnter) {
				if (objectToCollide) {
					if (col.gameObject == objectToCollide) {
						if (onTriggerEnterFunctionName != "" && parent) {
							if (useEvents) {
								callEvent (onTriggerEnterEvent);
							} else {
								parent.SendMessage (onTriggerEnterFunctionName, col.gameObject);
							}
							active = true;
						} else {
							active = true;
						}
					}
				} else {
					if (useEvents) {
						callEvent (onTriggerEnterEvent);
					} else {
						if (onTriggerEnterFunctionName != "" && parent) {		
							parent.SendMessage (onTriggerEnterFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
						}
						active = true;
					}
				}
			}
		} else {
			if (onTriggerExit) {
				active = true;
				if (useEvents) {
					callEvent (onTriggerExitEvent);
				} else {
					if (onTriggerExitFunctionName != "" && parent) {
						parent.SendMessage (onTriggerExitFunctionName, col.gameObject);
					}
				}
			}
		}
	}

	void OnTriggerStay (Collider col)
	{
		checkTriggerOnState (col);
	}

	public void checkTriggerOnState (Collider col)
	{
		if (onTriggerStay) {
			if (objectToCollide) {
				if (col.gameObject == objectToCollide) {
					if (onTriggerStayFunctionName != "" && parent) {
						if (useEvents) {
							callEvent (onTriggerStayEvent);
						} else {
							parent.SendMessage (onTriggerStayFunctionName, col.gameObject);
						}
						active = true;
					} else {
						active = true;
					}
				}
			} else {
				if (useEvents) {
					callEvent (onTriggerStayEvent);
				} else {
					if (onTriggerStayFunctionName != "" && parent) {
						parent.SendMessage (onTriggerStayFunctionName, col.gameObject);
					}
					active = true;
				}
			}
		}
	}

	public void checkTriggerWithGameObject (GameObject obj, bool isEnter)
	{
		if (isEnter) {
			if (onTriggerEnter) {
				if (objectToCollide) {
					if (obj == objectToCollide) {
						if (onTriggerEnterFunctionName != "" && parent) {
							if (useEvents) {
								callEvent (onTriggerEnterEvent);
							} else {
								parent.SendMessage (onTriggerEnterFunctionName, obj);
							}
							active = true;
						} else {
							active = true;
						}
					}
				} else {
					if (useEvents) {
						callEvent (onTriggerEnterEvent);
					} else {
						if (onTriggerEnterFunctionName != "" && parent) {	
							parent.SendMessage (onTriggerEnterFunctionName, obj, SendMessageOptions.DontRequireReceiver);
						}
						active = true;
					}
				}
			}
		} else {
			if (onTriggerExit) {
				active = true;
				if (useEvents) {
					callEvent (onTriggerExitEvent);
				} else {
					if (onTriggerExitFunctionName != "" && parent) {
						parent.SendMessage (onTriggerExitFunctionName, obj);
					}
				}
			}
		}
	}

	public void callEvent(UnityEvent eventToCall){
		if (eventToCall.GetPersistentEventCount () > 0) {
			eventToCall.Invoke ();
		}
	}
}