using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    //used to send data to objects listen for a pickup e.g. progress bar, sound manager
    public class PickupEventData : EventData
    {
        #region Variables
        private int pickupValue;
        private string targetID; //which health progress bar should be updated e.g. player1?
        #endregion

        #region Properties
        public int PickupValue
        {
            get
            {
                return pickupValue;
            }
        }
        public string TargetID
        {
            get
            {
                return targetID;
            }
        }
        #endregion

        public PickupEventData(string id, object sender, EventActionType eventType,
            EventCategoryType eventCategoryType, int pickupValue, string targetID)
            : base(id, sender, eventType, eventCategoryType)
        {
            this.pickupValue = pickupValue;           //e.g. 10 points for the pickup
            this.targetID = targetID;      //e.g. increase "player1" health
        }

        public override string ToString()
        {
            return base.ToString() + "PickupEventData - Value: " + this.pickupValue;
        }

        //add GetHashCode and Equals
        public override bool Equals(object obj)
        {
            PickupEventData other = obj as PickupEventData;
            return base.Equals(obj) && this.pickupValue == other.PickupValue && this.targetID.Equals(other.TargetID);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 11 + this.pickupValue.GetHashCode();
            hash = hash * 31 + this.targetID.GetHashCode();
            return hash;
        }
    }
}
