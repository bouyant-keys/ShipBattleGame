using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    // In C#, non-primitive types are automatically passed by reference.
    void SaveData(GameData data);

    void LoadData(GameData data);
}
