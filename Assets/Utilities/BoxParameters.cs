using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Utilities {
    class BoxParameters {
        public readonly float lowerXBoundary;
        public readonly float upperXBoundary;
        public readonly float lowerYBoundary;
        public readonly float upperYBoundary;
        public readonly float lowerZBoundary;
        public readonly float upperZBoundary;

        public readonly Vector3 position;
        public readonly Vector3 size;

        public float Volume => (upperXBoundary - lowerXBoundary) * (upperYBoundary - lowerYBoundary) * (upperZBoundary - lowerZBoundary);

        public BoxParameters(BoxCollider collider) {
            this.position = collider.transform.position;
            this.size = collider.size;
            lowerXBoundary = collider.transform.position.x - (collider.size.x / 2);
            upperXBoundary = collider.transform.position.x + (collider.size.x / 2);
            lowerYBoundary = collider.transform.position.y - (collider.size.y / 2);
            upperYBoundary = collider.transform.position.y + (collider.size.y / 2);
            lowerZBoundary = collider.transform.position.z - (collider.size.z / 2);
            upperYBoundary = collider.transform.position.z + (collider.size.z / 2);
        }

        public BoxParameters(Vector3 position, Vector3 size) {
            this.position = position;
            this.size = size;
            lowerXBoundary = position.x - (size.x / 2);
            upperXBoundary = position.x + (size.x / 2);
            lowerYBoundary = position.y - (size.y / 2);
            upperYBoundary = position.y + (size.y / 2);
            lowerZBoundary = position.z - (size.z / 2);
            upperYBoundary = position.z + (size.z / 2);
        }
        public static BoxParameters operator -(BoxParameters x, BoxParameters y) {
            return Substract(x, y);
        }

        public static BoxParameters Substract(BoxParameters from, BoxParameters substract) {
            float newLowerXBoundry = 0;
            if(substract.lowerXBoundary < from.lowerXBoundary && substract.upperXBoundary > from.lowerXBoundary && substract.lowerXBoundary < from.upperXBoundary && substract.upperXBoundary < from.upperXBoundary) {
                //Option 1
                newLowerXBoundry = substract.upperXBoundary;
            }
            if(substract.lowerXBoundary > from.lowerXBoundary && substract.upperXBoundary > from.lowerXBoundary && substract.lowerXBoundary < from.upperXBoundary && substract.upperXBoundary < from.upperXBoundary) {
                //Option 2
                newLowerXBoundry = from.lowerXBoundary;
            }
            if(substract.lowerXBoundary > from.lowerXBoundary && substract.upperXBoundary > from.lowerXBoundary && substract.lowerXBoundary < from.upperXBoundary && substract.upperXBoundary > from.upperXBoundary) {
                //Option 3
                newLowerXBoundry = from.lowerXBoundary;
            }
            if(substract.lowerXBoundary > from.lowerXBoundary && substract.upperXBoundary > from.lowerXBoundary && substract.lowerXBoundary < from.upperXBoundary && substract.upperXBoundary > from.upperXBoundary) {
                //Option 3
                newLowerXBoundry = from.lowerXBoundary;
            }
            if(substract.lowerXBoundary > from.lowerXBoundary && substract.upperXBoundary > from.lowerXBoundary && substract.lowerXBoundary < from.upperXBoundary && substract.upperXBoundary > from.upperXBoundary) {
                //Option 3
                newLowerXBoundry = from.lowerXBoundary;
            }
        }
    }
}
