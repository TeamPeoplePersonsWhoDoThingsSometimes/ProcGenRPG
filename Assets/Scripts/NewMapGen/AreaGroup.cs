using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaGroup {

    //Areas don't need to be in any order.
    private List<Area> areas;


    public AreaGroup()
    {
        areas = new List<Area>();
    }

    public void addArea(Area a)
    {
        areas.Add(a);
    }

    //Returns a random Area from this AreaGroup.
    private Area getRandomArea()
    {
        //TODO: Implement this method.
        return null;
    }
}
