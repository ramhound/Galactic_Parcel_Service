using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[RequireComponent(typeof(UILabel))]
public class ScrollingText : MonoBehaviour {
    /// <summary>
    /// The builder for appending the chars
    /// </summary>
    private StringBuilder builder;

    /// <summary>
    /// Used to manage the code tag listeners
    /// </summary>
    private Dictionary<string, CodeTagListener> codeTagListeners = new Dictionary<string, CodeTagListener>();
    /// <summary>
    /// The text asset used is setting this objects text 
    /// </summary>
    public TextAsset dialog;

    /// <summary>
    /// Scrolling speed will control how fast the text scrolls across the screen.
    /// The number supplied should be between 0f - 1f; 1 being instant display;
    /// </summary>
    [Range(0, 1)]
    public float scrollSpeed;

    /// <summary>
    /// Should the text start scrolling upon being created.
    /// </summary>
    public bool scrollOnStart;

    /// <summary>
    /// The underlying UILabel that will be used to display our text
    /// </summary>
    private UILabel _label;
    public UILabel label {
        get { return _label ?? (label = GetComponent<UILabel>()); }
        set { _label = value; }
    }

    /// <summary>
    /// The text that will be used for scrolling
    /// </summary>
    public string text { get; set; }

    /// <summary>
    /// Property for if the scrolling of text is finished
    /// </summary>
    private bool _scrollFinished = true;
    public bool scrollFinished { get { return _scrollFinished; } }

    /// <summary>
    /// Property for if we are currently scrolling text
    /// </summary>
    private bool _isScrolling;
    public bool isScrolling { get { return _isScrolling; } }

    public delegate void ScrollingTextDelegate(ScrollingText scrollingText);
    public delegate void CodeTagListener(string[] args);
    public delegate void CodetagListenerHub(string methodName, string[] args);
    //public delegate void OnCharacterPrint(char c);
    public ScrollingTextDelegate onScrollStart;
    public ScrollingTextDelegate onScrollFinish;
    public ScrollingTextDelegate onCharacterPrint;
    public CodetagListenerHub onCodeTag;
    //public OnCharacterPrint onCharacterPrint;

    private void Start() {
        if(scrollOnStart)
            StartScrolling();
    }

    private string CheckForUIColourTag() {
        if(index + 7 < text.Length && text[index + 7] == ']') {
            char[] hexCode = text.ToCharArray().RangeSubset(index, 8);
            index = index + 8;
            return new string(hexCode);
        }
        return string.Empty;
    }

    private string CheckForUIEndTag() {
        if(index + 2 < text.Length && text[index + 1] == '-' && text[index + 2] == ']') {
            char[] endTag = text.ToCharArray().RangeSubset(index, 3);
            index = index + 3;
            return new string(endTag);
        }
        return string.Empty;
    }

    private void CheckForCodeTag() {
        int methodIndex = index + 1;
        int argsIndex = 0;
        string methodName = string.Empty;

        //first attempt to pull out the method name
        for(int i = methodIndex; i < text.Length; i++) {
            if(text[i] == '(') {
                argsIndex = i + 1;
                methodName = new string(text.ToCharArray().RangeSubset(methodIndex, i - methodIndex));
                break;
            }
        }

        //if no method name was found there was a ScriptFormatError; was going throw but decided to just let it print
        if(string.IsNullOrEmpty(methodName)) return;

        //handle in the arguments
        for(int i = argsIndex; i < text.Length; i++) {
            if(text[i] == ')') {
                string tempArgs = new string(text.ToCharArray().RangeSubset(argsIndex, i - argsIndex));
                string[] args = tempArgs.Replace(" ", string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if(onCodeTag != null) onCodeTag(methodName, args);
                CodeTagListener method;
                codeTagListeners.TryGetValue(methodName.ToLower(), out method);
                if(method != null) method(args);
                break;
            }
        }

        //move the index past the code block and remove the code tag
        for(int i = index + 1; i < text.Length; i++) {
            if(text[i] == '}') {
                if(!isScrolling) text = text.Remove(index, i - index + 1);
                index = i + 1;
                break;
            }
        }
        if(index < text.Length && text[index] == '{') CheckForCodeTag();
    }

    private void Refresh() {
        if(string.IsNullOrEmpty(text)) {
            if(dialog != null) {
                text = dialog.RemoveComments();
            } else if(!string.IsNullOrEmpty(label.text)) {
                text = label.text;
            } else {
                text = "Hello World";
            }
        }

        label.text = string.Empty;
        builder = new StringBuilder(text.Length);
        index = 0;
    }

    int index = 0;
    private IEnumerator ScrollText() {
        _isScrolling = true;
        while(index < text.Length) {
            if(text[index] == '[') {
                string tag = CheckForUIColourTag();
                if(string.IsNullOrEmpty(tag)) tag = CheckForUIEndTag();
                builder.Append(tag);
            } else if((onCodeTag != null || codeTagListeners.Count > 0) && text[index] == '{') CheckForCodeTag();
            if(index < text.Length) builder.Append(text[index]);
            label.text = builder.ToString();
            if(onCharacterPrint != null) onCharacterPrint(this);
            index++;
            yield return new WaitForSeconds((1f - scrollSpeed) / 5);
        }

        ScrollFinished();
    }

    /// <summary>
    /// Starts the scrolling of the text. If paused; will resume
    /// </summary>
    public void StartScrolling() {
        if(!isScrolling) {
            if(_scrollFinished) {
                _scrollFinished = false;
                Refresh();
            }

            StartCoroutine("ScrollText");
            if(onScrollStart != null)
                onScrollStart(this);
        }
    }

    /// <summary>
    /// Stops the scrolling and instantly displays the text.
    /// </summary>
    public void StopScrolling() {
        _isScrolling = false;
        StopCoroutine("ScrollText");
        for(; index < text.Length; index++) {
            if(text[index] == '{') { 
                CheckForCodeTag();
            }
        }
        label.text = text;
        ScrollFinished();
    }

    /// <summary>
    /// Pauses the scrolling of text. 
    /// </summary>
    public void PauseScrolling() {
        _isScrolling = false;
        StopCoroutine("ScrollText");
    }

    private void ScrollFinished() {
        text = string.Empty;
        index = 0;
        _isScrolling = false;
        _scrollFinished = true;

        if(onScrollFinish != null)
            onScrollFinish(this);
    }

    /// <summary>
    /// Registers a code tag listener by a specific name. This is not case sensitive
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="listener"></param>
    public void RegisterCodeTagListener(string methodName, CodeTagListener listener) {
        //if(methodName.EqualsIgnoreCase("Choice") || methodName.EqualsIgnoreCase("Switch"))
        //    throw new Exception("You cannot register a code tag listener by that name: " + methodName + "\nIt is reserved.");
        codeTagListeners.Add(methodName.ToLower(), listener);
    }

    /// <summary>
    /// Removes a code tag lister by name. This is not case sensitive
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="listener"></param>
    public void RemoveCodeTagListener(string methodName, CodeTagListener listener) {
        //if(methodName.EqualsIgnoreCase("Choice") || methodName.EqualsIgnoreCase("Switch"))
        //    throw new Exception("You cannot remove a code tag listener by that name: " + methodName + "\nIt is reserved.");
        methodName = methodName.ToLower();
        if(codeTagListeners.ContainsKey(methodName))
            codeTagListeners.Remove(methodName);
    }
}

public class ScriptFormatException : Exception {
    public ScriptFormatException() : base() { }
    public ScriptFormatException(string reason) : base(reason) { }
}

