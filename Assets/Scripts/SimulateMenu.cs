using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateMenu : MonoBehaviour
{
    //スクロールビュー内のコンテンツの数
    /// <summary>
    /// MENU_IDX: MENUコンテンツ数, MENU_MARGIN: MENUマージン, MENU_WIDTH: MENU幅, MENU_HEIGHT: MENU高さ
    /// </summary>
	private const int 
    MENU_IDX = 5,
	MENU_WIDTH = 100,
	MENU_HEIGHT = 25;
	private int _menuMarginX()
	{
		return (Screen.width - MENU_WIDTH) / 2 ;
	}
	private int _menuMarginY()
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
	private Rect _contSize()
	{
		return new Rect(_menuMarginX(), MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
	} 
	// Rect _contSize = new Rect(MENU_MARGIN_X, MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
    /// <summary>
    /// コンテンツ配列
    /// </summary>
	/// <remarks>
	/// Rect 引数(矩形左上のx, y, 幅, 高さ)
	/// </remarks>
	private Rect[] _contList = new Rect[MENU_IDX];
    /// <summary>
    /// スクロールの位置更新
    /// </summary>
	private Vector2 _scrollPosition = Vector2.zero;
	/// <summary>
	/// _viewPosition: スクロールビュー表示範囲, _wholePosition: スクロールビュー全体の範囲
	/// </summary>
	private Rect _viewPosition()
	{
		return new Rect(_menuMarginX(), _menuMarginY() + (MENU_HEIGHT * 2.5f), MENU_WIDTH + _menuMarginX(), MENU_HEIGHT * (MENU_IDX - 2));
	}
	private Rect _wholePosition()
	{
		return new Rect(_menuMarginX(), _menuMarginY() + (MENU_HEIGHT * 2), MENU_WIDTH, MENU_HEIGHT * (MENU_IDX + 1));
	}
	/// <summary>
	/// スクロールビュー表示切替え
	/// </summary>
	private bool _showMenu;
	private int _clickCnt = 0;                                                  // クリック回数


    void OnGUI()
    {   
		GUIStyle stylebutton = GUI.skin.GetStyle("button"); 		// ボタンとボックスがGUIStyleになるように設定
		GUIStyle stylebox = GUI.skin.GetStyle("box");
		
		stylebutton.fontSize = 14;									// ボタンとボックスの fontSize変更
		stylebox.fontSize = 20;
		int idx = 0;

		if(GUI.Button
			(new Rect(
				// MENU_MARGIN_X +50, 
				// MENU_MARGIN_Y + (MENU_HEIGHT * (MENU_IDX-4)), 
				_menuMarginX(), 
				_menuMarginY() + (MENU_HEIGHT * (MENU_IDX-4)), 
				MENU_WIDTH, 
				MENU_HEIGHT * 1.5f), 
			$"DEBUG\n_clickCnt: {_clickCnt}"))   					// スクロールビュー表示切替えボタン
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

		if(GUI.Button(_contList[++idx], ">"))						// 進むボタン
		{
			Time.timeScale += 0.5f;
			_clickCnt++;
		}
		
		if(GUI.Button(_contList[++idx], "<"))						// 戻るボタン
		{
			if(Time.timeScale > 0)
				// Time.timeScale -= 0.5f;
				_clickCnt--;
		}

		if(GUI.Button(_contList[++idx], "x1"))						// BACK to MAIN ボタン
		{
			// Time.timeScale = 1;
		}
		
		GUI.EndScrollView();										// スクロールビューの終了位置
    }

    private void Start()
    {        
		ScrollContents();											// スクロールコンテンツ描画
    }

	/// <summary>
	/// スクロールコンテンツ描画
	/// </summary>
    private void ScrollContents()
    {
        int marginY = _menuMarginY();								// コンテンツボタン開始位置(_MenuMarginY)の再計算
        for (int i = 0; i < MENU_IDX; i++)
        {
            _contList[i] = _contSize();
 			_contList[i].y = marginY + (MENU_HEIGHT * (i + 1));
        }
    }
}