using RimWorld;

namespace MGAutoSell.AI
{
    public class FloatMenuOptionProvider_Barter : FloatMenuOptionProvider
    {
        protected override bool Drafted => false;
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;

        //protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        //{
        //    //if(clickedThing != )
        //}
    }
}
