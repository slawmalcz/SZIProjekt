using UnityEngine;

namespace Assets.Utilities {
    public class BoxParameters {
        public readonly float lowerXBoundary;
        public readonly float upperXBoundary;
        public readonly float lowerYBoundary;
        public readonly float upperYBoundary;
        public readonly float lowerZBoundary;
        public readonly float upperZBoundary;

        public readonly Vector3 position;
        public readonly Vector3 size;

        public float Volume => (upperXBoundary - lowerXBoundary) * (upperYBoundary - lowerYBoundary) * (upperZBoundary - lowerZBoundary);

        public BoxParameters(Vector3 position, Vector3 size) {
            this.position = position;
            this.size = size;
            lowerXBoundary = position.x - (size.x / 2);
            upperXBoundary = position.x + (size.x / 2);
            lowerYBoundary = position.y - (size.y / 2);
            upperYBoundary = position.y + (size.y / 2);
            lowerZBoundary = position.z - (size.z / 2);
            upperZBoundary = position.z + (size.z / 2);
        }

        public BoxParameters(BoxCollider collider) : this(collider.transform.position, collider.transform.lossyScale) { }

        public BoxParameters(float lowerXBoundary, float upperXBoundary, float lowerYBoundary, float upperYBoundary, float lowerZBoundary, float upperZBoundary) :
            this(new Vector3((lowerXBoundary + upperXBoundary) / 2, (lowerYBoundary + upperYBoundary) / 2, (lowerZBoundary + lowerZBoundary) / 2),
                  new Vector3((upperXBoundary - lowerXBoundary), (upperYBoundary - lowerYBoundary), (upperZBoundary - lowerZBoundary))) { }

        public static BoxParameters operator -(BoxParameters x, BoxParameters y) => Substract(x, y);

        public static BoxParameters Substract(BoxParameters from, BoxParameters substract) {
            float newLowerXBoundry = 0;
            float newUpperXBoundry = 0;
            ClipingBoundries(from.lowerXBoundary, from.upperXBoundary, substract.lowerXBoundary, substract.upperXBoundary, ref newLowerXBoundry, ref newUpperXBoundry);
            float newLowerYBoundry = 0;
            float newUpperYBoundry = 0;
            ClipingBoundries(from.lowerYBoundary, from.upperYBoundary, substract.lowerYBoundary, substract.upperYBoundary, ref newLowerYBoundry, ref newUpperYBoundry);
            float newLowerZBoundry = 0;
            float newUpperZBoundary = 0;
            ClipingBoundries(from.lowerZBoundary, from.upperZBoundary, substract.lowerZBoundary, substract.upperZBoundary, ref newLowerZBoundry, ref newUpperZBoundary);
            return new BoxParameters(newLowerXBoundry, newUpperXBoundry, newLowerYBoundry, newUpperYBoundry, newLowerZBoundry, newUpperZBoundary);
        }

        private static void ClipingBoundries(float fromLowerBoundry, float fromUpperBoundry, float sunstractLowerBoundry, float substractUpperBoundry, ref float newLower, ref float newUpper) {
            if(sunstractLowerBoundry < fromLowerBoundry && substractUpperBoundry > fromLowerBoundry &&
               sunstractLowerBoundry < fromUpperBoundry && substractUpperBoundry < fromUpperBoundry) {
                //Option 1
                newLower = substractUpperBoundry;
                newUpper = fromUpperBoundry;
            } else if(sunstractLowerBoundry > fromLowerBoundry && substractUpperBoundry > fromLowerBoundry &&
                      sunstractLowerBoundry < fromUpperBoundry && substractUpperBoundry > fromUpperBoundry) {
                //Option 3
                newLower = fromLowerBoundry;
                newUpper = sunstractLowerBoundry;
            } else {
                //Option 4
                newLower = fromLowerBoundry;
                newUpper = fromUpperBoundry;
            }
        }
    }
}
