using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaGroup {

    //TODO: Create a function that randomly assigns a quest to an Area in this AreaGroup.
    
    //TODO: Create a function that gives this AreaGroup a name, based on what kind of AreaGroup it is.

    //TODO: Create some variable that represents a type of Area. Like a Biome.

    public AreaType type;

    private string name;

    //Areas don't need to be in any order.
    private List<Area> areas;

    public AreaGroup(AreaType type)
    {
        this.type = type;

        areas = new List<Area>();
    }

    //Adds the Area to this AreaGroup, and sets it's type to be this group's type.
    public void addArea(Area a)
    {
        areas.Add(a);
        a.setGroup(this);
    }

    //Returns a random Area from this AreaGroup.
    private Area getRandomArea()
    {
        //TODO: Implement this method.
        return null;
    }

    public string getName()
    {
        return name;
    }
}
