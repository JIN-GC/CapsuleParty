using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //スクロールビュー内のコンテンツの数
    /// <summary>
    /// MENU_IDX: MENUコンテンツ数, MENU_MARGIN: MENUマージン, MENU_WIDTH: MENU幅, MENU_HEIGHT: MENU高さ
    /// </summary>
	private const int 
    MENU_IDX = 5,
	// MENU_MARGIN_X = 80,
	// MENU_MARGIN_Y = 200,
	MENU_WIDTH = 100,
	MENU_HEIGHT = 25;
	int _menuMarginX()
	{
		return (Screen.width - MENU_WIDTH) / 2 ;
	}
	int _menuMarginY()
	{
		// return (Screen.height - MENU_HEIGHT) / 2;
		return Screen.height - (MENU_HEIGHT * (MENU_IDX + 1));
	}
    /// <summary>
    /// コンテンツ規格サイズ
    /// </summary>
	/// <remarks>
	/// Rect 引数(矩形左上のx, y, 幅, 高さ)
	/// </remarks>
	Rect _contSize()
	{
		return new Rect(_menuMarginX(), MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
		// return new Rect(MENU_MARGIN_X+50, MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
	} 
	// Rect _contSize = new Rect(MENU_MARGIN_X, MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
    /// <summary>
    /// コンテンツ配列
    /// </summary>
	Rect[] _contList = new Rect[MENU_IDX];
    /// <summary>
    /// スクロールの位置更新
    /// </summary>
	Vector2 _scrollPosition = Vector2.zero;
	/// <summary>
	/// _viewPosition: スクロールビュー表示範囲, _wholePosition: スクロールビュー全体の範囲
	/// </summary>
	/// <remarks>
	/// Rect 引数(矩形左上のx, y, 幅, 高さ)
	/// </remarks>
	Rect _viewPosition()
	{
		return new Rect(_menuMarginX(), _menuMarginY() + (MENU_HEIGHT * 2.5f), MENU_WIDTH + _menuMarginX(), MENU_HEIGHT * (MENU_IDX - 2));
		// return new Rect(MENU_MARGIN_X+50, (MENU_MARGIN_Y + (MENU_HEIGHT * 2.5f)), MENU_WIDTH + MENU_MARGIN_X, MENU_HEIGHT * (MENU_IDX-2));
	}
	// Rect _viewPosition = new Rect(MENU_MARGIN_X, MENU_MARGIN_Y + (MENU_HEIGHT * 2), MENU_WIDTH + MENU_MARGIN_X, MENU_HEIGHT * 4);
	Rect _wholePosition()
	{
		return new Rect(_menuMarginX(), _menuMarginY() + (MENU_HEIGHT * 2), MENU_WIDTH, MENU_HEIGHT * (MENU_IDX + 1));
		// return new Rect(MENU_MARGIN_X+50, (MENU_MARGIN_Y - (MENU_HEIGHT * 2.5f)), MENU_WIDTH, MENU_HEIGHT * (MENU_IDX+1));
	}
	// Rect _wholePosition = new Rect(MENU_MARGIN_X, MENU_MARGIN_Y - (MENU_HEIGHT * (MENU_MARGIN_Y / (MENU_HEIGHT * 2))), MENU_WIDTH, MENU_HEIGHT * (MENU_IDX+2));
	/// <summary>
	/// スクロールビュー表示切替え
	/// </summary>
	bool _showMenu;

    void OnGUI()
    {          
		GUIStyle stylebutton = GUI.skin.GetStyle("button"); 		// ボタンとボックスがGUIStyleになるように設定
		GUIStyle stylebox = GUI.skin.GetStyle("box");
		
		stylebutton.fontSize = 14;									// ボタンとボックスの fontSize変更
		stylebox.fontSize = 20;
		int idx = 0;

        // if ( GUI.Button(new Rect((Screen.width/2), (Screen.height/2), 100, 40), "Button") ) {
        //     Debug.Log ("clicked!");
        // }

		if(GUI.Button
			(new Rect(
				// MENU_MARGIN_X +50, 
				// MENU_MARGIN_Y + (MENU_HEIGHT * (MENU_IDX-4)), 
				_menuMarginX(), 
				_menuMarginY() + (MENU_HEIGHT * (MENU_IDX-4)), 
				MENU_WIDTH, 
				MENU_HEIGHT * 1.5f), 
			// $"MENU"))   											// スクロールビュー表示切替えボタン
			$" MENU \n W:{Screen.width} H:{Screen.height} "))   // スクロールビュー表示切替えボタン
			{
			_showMenu = !_showMenu;
            Debug.Log ("_showMenu!");
            if (_showMenu)
            {
                ScrollContents();									// スクロールコンテンツ描画
            }
		}
		if(!_showMenu) return;

		_scrollPosition = GUI.BeginScrollView(_viewPosition(), _scrollPosition, _wholePosition());  //スクロールビューの開始位置

		if(GUI.Button(_contList[++idx], "PLAY"))					// 開始ボタン
		{
			SceneManager.LoadScene("1206");
		}
		
		if(GUI.Button(_contList[++idx], "SIMULATION"))				// シュミレーションボタン
		{
			SceneManager.LoadScene("Simulation");
		}
		
		if(GUI.Button(_contList[++idx], "END"))						// 終了ボタン
		{
            // Application.Quit();
			SceneManager.LoadScene("End");
		}

		GUI.EndScrollView();										// スクロールビューの終了位置
        
        // Rect rect2 = new Rect(0, 25, 250, 200);					// 引数(矩形左上のx, y, 幅, 高さ)
        // _styleState.textColor = Color.white;
        // _style.normal = _styleState;
        // GUI.Label(rect2, _uiInfo2, _style);						// GUIテキスト表示
        // _style.fontSize = 13;
        // if (_clickCnt >= _verticesSubList.Count)
        // {   
        //     Rect rect3 = new Rect((Screen.width/2)-80, (Screen.height/2)-160, 40, 20);	// 引数(矩形左上のx, y, 幅, 高さ)
        //     _styleStateFinal.textColor = Color.magenta;
        //     _styleFinal.normal = _styleStateFinal;
        //     _styleFinal.fontStyle = FontStyle.Bold;
        //     _styleFinal.fontSize = 32;
        //     GUI.Label(rect3, "COMPLETED", _styleFinal);			// GUIテキスト表示
        // }
    }

    private void Start()
    {
        // _style = new GUIStyle();
        // _styleFinal = new GUIStyle();
        // _styleState = new GUIStyleState();
        // _styleStateFinal = new GUIStyleState();
        // コンテンツのサイズを代入、位置Yを更新して並べる
		ScrollContents();											// スクロールコンテンツ描画
    }
	/// <summary>
	/// スクロールコンテンツ描画
	/// </summary>
    private void ScrollContents()
    {
        int marginY = _menuMarginY();								// _MenuMarginY()値の再計算
        for (int i = 0; i < MENU_IDX; i++)
        {
            _contList[i] = _contSize();
            // _contList[i].y *= i + 1;
			_contList[i].y = marginY + (MENU_HEIGHT * (i + 1));
        }
    }
}
