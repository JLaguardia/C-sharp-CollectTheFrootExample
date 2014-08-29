using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	//Author: James Laguardia
	//This is the Android/mobile version of Collect The Froot.
	//public Transform r, l;

	private bool grabbedBasket = false, tappedOnce = false;
	public static bool gameNotOver, gamePaused;
	private float distanceFromGround, distanceFromCam, detectionTime = 0, accumTime = 0.0f, scrnWidth, scrnHeight, pxOffset;
	public static float xBoundLeft, xBoundRight;
	//public static int difficulty;
	private Vector3 movement;
	private Texture2D[] guiTx;
	public Texture2D[] otherTx;
	private GUIStyle styleA, styleB;
	
	void Start () {
		init();
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(scrnWidth - 64, 0, 64, 64), otherTx[2]);
		if(gamePaused){
			//
		} else{
			if(gameNotOver){
				styleA.normal.background = guiTx[0];
				styleB.normal.background = null;
				
				//score display
				styleB.normal.textColor = Color.white;
				styleB.fontSize = 15;
				GUI.Label(new Rect(0, 0, 255, 20), "Score: " + Player.currentScore + "  | TopScore: " + Player.topScore, styleB);
				//GUI.Label(new Rect(scrnWidth / 2 - 50, 0, 100, 20), "Juncture " + Player.currentScore, styleB);
				
				//life display (hearts?)
				GUI.Label(new Rect(0, 10, 50, 20), "Life: ");
				GUI.DrawTexture(new Rect(30, 15, 10, 10), otherTx[1], ScaleMode.ScaleToFit, true, 1);
				GUI.DrawTexture(new Rect(40, 15, 10, 10), otherTx[0], ScaleMode.ScaleToFit, true, 1);
				GUI.DrawTexture(new Rect(50, 15, 10, 10), otherTx[0], ScaleMode.ScaleToFit, true, 1);
				if(Player.currentLife > 1){
					GUI.DrawTexture(new Rect(40, 15, 10, 10), otherTx[1], ScaleMode.ScaleToFit, true, 1);
				}
				if(Player.currentLife > 2){
					GUI.DrawTexture(new Rect(50, 15, 10, 10), otherTx[1], ScaleMode.ScaleToFit, true, 1);
				}

				//score multiplier bar
				GUI.Label(new Rect(0, 20, 60, 20), "Multiplier: ");
				GUI.Label(new Rect(pxOffset + 20, 40, 100, 40), "x" + Player.currentMultiplier, styleB);

				switch((int)Player.currentMultiplier){
				case 1:
					styleA.normal.background = guiTx[0];
					styleB.normal.background = guiTx[3];
					break;
				case 2:
					styleA.normal.background = guiTx[3];
					styleB.normal.background = guiTx[4];
					break;
				case 3:
					styleA.normal.background = guiTx[4];
					styleB.normal.background = guiTx[5];
					break;
				case 4:
					styleA.normal.background = guiTx[5];
					styleB.normal.background = guiTx[6];
					break;
				case 5:
					styleA.normal.background = guiTx[6];
					styleB.normal.background = guiTx[7];
					break;
				case 6:
					styleA.normal.background = guiTx[7];
					styleB.normal.background = guiTx[8];
					break;
				case 7:
					styleA.normal.background = guiTx[8];
					styleB.normal.background = guiTx[9];
					break;
				case 8:
					styleA.normal.background = guiTx[9];
					styleB.normal.background = guiTx[10];
					break;
				case 9:
					styleA.normal.background = guiTx[10];
					styleB.normal.background = guiTx[11];
					break;
				case 10:
					styleA.normal.background = guiTx[11];
					break;
				}

				//backgroundbox
				GUI.Box(new Rect(5, 50, 20, 150), "", styleA);
				//foregroundbox
				GUI.Box(new Rect(5, 200, 20, -(Player.mltHeight * 150)), "", styleB);
				
			} else{
				//game over
				GUIStyle style2 = new GUIStyle();
				style2.normal.textColor = Color.red;
				style2.fontStyle = FontStyle.Bold;
				style2.fontSize = 22;
				style2.wordWrap = true;
				GUI.Label(new Rect(scrnWidth / 10, scrnHeight / 2, 400, 200), "Game Over! Your score: " + Player.currentScore, style2);
				GUI.Label(new Rect(scrnWidth / 10, scrnHeight / 2 + 50, scrnWidth - (scrnWidth/10), 200), "Touch to return to the title screen.", style2);
			}
		}
	}
	
	private void gameFinish(){
		Time.timeScale = 0;
		if(Player.currentScore > Player.topScore){
			PlayerPrefs.SetInt("topscore", Player.currentScore);
		}
	}

	void Update(){
		
		if(Input.touchCount > 0){
			if(Input.GetTouch(0).phase == TouchPhase.Began){
				OnTouchDown(Input.GetTouch(0).position);
			} 
			if(Input.GetTouch(0).phase == TouchPhase.Moved){
				OnTouchMove(Input.GetTouch(0).position);
			}
			if(Input.GetTouch(0).phase == TouchPhase.Ended){
				OnTouchEnd(Input.GetTouch(0).position);
			}
			if(Input.GetTouch(0).phase == TouchPhase.Stationary){
				OnTouchStay(Input.GetTouch(0).position);
			}
		}
		
		
		if(Input.GetMouseButtonDown(0)){
			OnTouchDown(Input.mousePosition);
		}
		if(Input.GetMouseButton(0)){
			OnTouchMove(Input.mousePosition);
		}
		if(Input.GetMouseButtonUp(0)){
			OnTouchEnd(Input.mousePosition);
		}

		if(gamePaused){
			if(Time.timeScale > 0){
				Time.timeScale = 0;
				AudioListener.volume = 0;
			}
			//tintscreen
		} else {
			if(Time.timeScale < 1){
				Time.timeScale = 1;
				AudioListener.volume = OptionsManager.masterVol;
			}
		}
	}
	
	void FixedUpdate () {


			//to detect double tap
			if(tappedOnce){
				detectionTime += Time.deltaTime;
				if(detectionTime > 0.29f){
					detectionTime = 0;
					tappedOnce = false;
				}
			}
		
			if(Player.currentLife <= 0){
				gameNotOver = false;
				gameFinish();
			}

			Player.runtime += Time.deltaTime;
			accumTime += Time.deltaTime;
			if(accumTime > 60){//every 60 seconds: speed increases by 30%...
				Player.fallSpeed *= 1.3f;
				accumTime = 0;
			}

			if(movement.x < xBoundLeft){
				movement = new Vector3(xBoundLeft, distanceFromGround, distanceFromCam);
			} else if(movement.x > xBoundRight){
				movement = new Vector3(xBoundRight, distanceFromGround, distanceFromCam);
			}
			transform.position = movement;
	}


	#region TOUCH INPUT HANDLERS
	void OnTouchDown(Vector2 pos){
		if(!gameNotOver){
			Time.timeScale = 1;
			Application.LoadLevel(0);
		} else if(!gamePaused){
			Vector3 grabPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, distanceFromCam));
			if(grabPos.x <= transform.position.x && grabPos.x >= transform.position.x - 0.5f && grabPos.y < -0.5f){
				grabbedBasket = true;

				//detect a double tap... not sure how to implement this
				tappedOnce = true;
				if(detectionTime > 0.5f){
					tappedOnce = false;
					detectionTime = 0;
				}
			}
		}
	}
	
	void OnTouchMove(Vector2 updatePos){
		if(!gamePaused && grabbedBasket){
			updatePos.y = Camera.main.ScreenToWorldPoint(new Vector3(updatePos.x, distanceFromGround, distanceFromCam)).x;
			movement.x = updatePos.y + 0.25f;//reason for .25: compensate for model scale.
		}
	}

	void OnTouchEnd(Vector2 endPos){
		if(endPos.x >= scrnWidth - 64 && endPos.y >= scrnHeight - 64){
			gamePaused = !gamePaused;
		}
		grabbedBasket = false;
	}

	void OnTouchStay(Vector2 stayPos){
		//??
	}
	#endregion

	//Do I need this?
	public static bool isMobile(){
		return Application.platform == RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer;
	}
	
	private void init(){
		gamePaused = false;
		distanceFromCam =  transform.position.z - Camera.main.transform.position.z;
		distanceFromGround = transform.position.y;// - GameObject.Find("ground").transform.position.y;
		setupTextures();
		scrnWidth = Screen.width;
		scrnHeight = Screen.height;
		pxOffset = scrnWidth / 20;
		Player.topScore = PlayerPrefs.GetInt("topscore");
		gameNotOver = true;
		Time.timeScale = 1;
		Player.currentLife = 3;
		Player.currentScore = 0;
		Player.fallSpeed = 0.5f;
		Player.runtime = 0;
		Player.currentMultiplier = 1;
		Player.mltHeight = 0;
		movement = transform.position;
		xBoundLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceFromCam)).x;
		xBoundRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceFromCam)).x;

		//l.position = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceFromCam));
		//r.position = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceFromCam));


		styleA = new GUIStyle();
		styleB = new GUIStyle();
	}
	
	private void setupTextures(){
		//gray, cyan, red, heartblank, heartfull, green, oj, lav, blue, dred, yellow, purple
		guiTx = new Texture2D[12];
		guiTx[0] = new Texture2D(1,1);
		guiTx[0].SetPixels(new Color[] {Color.gray});
		guiTx[0].Apply();
		guiTx[1] = new Texture2D(1,1);
		guiTx[1].SetPixels(new Color[] {Color.cyan});
		guiTx[1].Apply();
		guiTx[2] = new Texture2D(1,1);
		guiTx[2].SetPixels(new Color[] {Color.red});
		guiTx[2].Apply();
		//guiTx 1 and 2 for turbo ////unused in mobile version
		guiTx[3] = new Texture2D(1,1);//yellow
		guiTx[3].SetPixels(new Color[] {Color.yellow});
		guiTx[3].Apply();
		guiTx[4] = new Texture2D(1,1);//d goldenrod
		guiTx[4].SetPixels(new Color[] {new Color(184.0f/255.0f, 134.0f/255.0f, 11.0f/255.0f, 1)});
		guiTx[4].Apply();
		guiTx[5] = new Texture2D(1,1);//orange
		guiTx[5].SetPixels(new Color[] {new Color(255.0f/255.0f, 165.0f/255.0f, 0, 1)});
		guiTx[5].Apply();
		guiTx[6] = new Texture2D(1,1);//dorange
		guiTx[6].SetPixels(new Color[] {new Color(255.0f/255.0f, 140.0f/255.0f, 0, 1)});
		guiTx[6].Apply();
		guiTx[7] = new Texture2D(1,1);//green yellow
		guiTx[7].SetPixels(new Color[] {new Color(173.0f/255.0f, 255.0f/255.0f, 47.0f/255.0f, 1)});
		guiTx[7].Apply();
		guiTx[8] = new Texture2D(1,1);//forest green
		guiTx[8].SetPixels(new Color[] {new Color(34.0f/255.0f, 139.0f/255.0f, 34.0f/255.0f, 1)});
		guiTx[8].Apply();
		guiTx[9] = new Texture2D(1,1);//orchid
		guiTx[9].SetPixels(new Color[] {new Color(218.0f/255.0f, 112.0f/255.0f, 214.0f/255.0f, 1)});
		guiTx[9].Apply();
		guiTx[10] = new Texture2D(1,1);//med orchid
		guiTx[10].SetPixels(new Color[] {new Color(186.0f/255.0f, 85.0f/255.0f, 211.0f/255.0f, 1)});
		guiTx[10].Apply();
		guiTx[11] = new Texture2D(1,1);//purple
		guiTx[11].SetPixels(new Color[] {new Color(160.0f/255.0f, 32.0f/255.0f, 240.0f/255.0f, 1)});
		guiTx[11].Apply();
		
	}
}
