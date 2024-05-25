using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextResizer : MonoBehaviour
{
    public TMP_Text[] _txtBoxes;

	void Start () 
	{
		StartCoroutine (SetTextSize ());	
	}
		
	IEnumerator SetTextSize ()
	{
		//Need to delay a frame
		yield return null;

		float fontSize = 1000;

		//Find smallest text size from instances
		for (int i = 0; i < _txtBoxes.Length; i++)
		{
			if (fontSize > _txtBoxes[i].fontSize)
            {
                fontSize = _txtBoxes[i].fontSize;
            }
        }

        //Set all to same smallest size
        for (int i = 0; i < _txtBoxes.Length; i++)
        {
            _txtBoxes[i].fontSizeMin = fontSize;
        }
    }
}
