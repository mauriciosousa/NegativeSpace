using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageSeparators
{
    public const char L0 = '$'; // header separator
    public const char L1 = '#'; // top level separator -> bodies
    public const char L2 = '/'; // -> body attributes
    public const char L3 = ':'; // -> 3D values
    public const char SET = '=';
}
