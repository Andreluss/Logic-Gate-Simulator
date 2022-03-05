using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectButtonCollision : CollisionData
{
    public override BaseRenderer Renderer { get => GetComponent<BaseRenderer>(); }
    public int ProjectID { get; set; }
    public string ProjectName { get => AppSaveData.Projects[ProjectID].defaultName; }
}
