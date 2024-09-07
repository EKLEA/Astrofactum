using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InfoDataBase
{
  public static DataBase<string, ItemInfo> ItemsDataBase;
  public static void InitBases()
  {
    ItemsDataBase= new DataBase<string,ItemInfo>("Items", item => item.ItemID);
  }
}
