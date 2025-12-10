using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowTextWindow : MonoBehaviour
{
    const string ANIM_FLAG_OPEN = "open";

    [SerializeField]
    Animator anim;

    [SerializeField]
    TMP_Text textObj;

    //[SerializeField]
    //TextMeshProUGUI textObj;

    [SerializeField]
    int delaySpeed = 100;

    //テキスト表示の折り返し数の指定(10なら10文字ごとに折り返し)
    [SerializeField]
    int wrapCount = 10;

    public void SetText(string text)
    {
        if (textObj != null)
        {
            textObj.text = text;
        }
        ShowText().Forget();
    }

    async UniTask ShowText()
    {
        // テキスト全体の長さ
        var length = textObj.text.Length;

        for (var i = 0; i < length; i++)
        {
            // 徐々に表示文字数を増やしていく
            textObj.maxVisibleCharacters = i;

            // 一定時間待機
            await UniTask.Delay(delaySpeed);
        }

        // 演出が終わったら全ての文字を表示する
        textObj.maxVisibleCharacters = length;
        // 指定文字数で改行させる
        string original = textObj.text;
        //textObj.text = InsertLineBreaks(original, wrapCount);
    }


    public void Open()
    {
        if (anim != null)
        {
            anim.SetBool(ANIM_FLAG_OPEN, true);
        }
    }
    public void Close()
    {
        if (anim != null)
        {
            anim.SetBool(ANIM_FLAG_OPEN, false);
        }
    }

    private string InsertLineBreaks(string input, int count)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++) {
            sb.Append(input[i]);
            //指定文字数ごとに改行挿入
            if((i + 1) % count == 0)
            {
                sb.Append("\n");
            }
        }

        return sb.ToString();
    }

}
