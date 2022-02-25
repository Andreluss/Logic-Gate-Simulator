using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultibitController : Node
{
    public int BitCount { get; private set; }
    public MultibitController(int bitCount, bool hidden) : base(0, 0, "InputGroup", hidden)
    {
        BitCount = bitCount;
    }
}
