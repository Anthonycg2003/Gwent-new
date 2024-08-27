using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ZoneInterface
{
    void OnTransformChildrenChanged();
    void SyncWithList();
}
