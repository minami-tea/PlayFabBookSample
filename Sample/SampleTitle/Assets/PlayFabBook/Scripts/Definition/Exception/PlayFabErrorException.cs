using PlayFab;
using System;

/// <summary>
/// 想定外の PlayFab のエラーを例外として扱うための Exception。
/// </summary>
public class PlayFabErrorException : Exception
{
    public PlayFabErrorException(PlayFabError error) : base(error.GenerateErrorReport())
    { }
}