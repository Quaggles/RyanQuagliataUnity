using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.TextMeshPro.Effects {
    [ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class TextWarp : MonoBehaviour {
        public TextMeshProUGUI TextMeshProUGUI;
        
        [SerializeReference, ShowInInspector, InlineProperty, OnValueChanged(nameof(UpdateVertices), true)]
        public ITextWarpFunction TextWarpFunction;

        /// <summary>
        /// True if the text must be updated at this frame 
        /// </summary>
        private bool m_forceUpdate;

        void Awake() {
            if (!TextMeshProUGUI) TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable() {
            m_forceUpdate = true;
        }

        private void OnRectTransformDimensionsChange() {
            UpdateVertices();
        }

        private void OnDisable() { }

        void Update() {
            if (!m_forceUpdate && !TextMeshProUGUI.havePropertiesChanged) return;
            UpdateVertices();
        }

        protected void UpdateVertices() {
            if (TextWarpFunction == null || !TextMeshProUGUI) return;
            m_forceUpdate = false;

            //during the loop, vertices represents the 4 vertices of a single character we're analyzing, 
            //while matrix is the roto-translation matrix that will rotate and scale the characters so that they will
            //follow the curve
            Vector3[] vertices;
            Matrix4x4 matrix;

            //Generate the mesh and get information about the text and the characters
            TextMeshProUGUI.ForceMeshUpdate();

            TMP_TextInfo textInfo = TextMeshProUGUI.textInfo;
            if (textInfo == null) return;
            int characterCount = textInfo.characterCount;

            //if the string is empty, no need to waste time
            if (characterCount == 0) return;

            //gets the bounds of the rectangle that contains the text 
            float boundsMinX = TextMeshProUGUI.bounds.min.x;
            float boundsMaxX = TextMeshProUGUI.bounds.max.x;

            //for each character
            for (int i = 0; i < characterCount; i++) {
                //skip if it is invisible
                if (!textInfo.characterInfo[i].isVisible) continue;

                //Get the index of the mesh used by this character, then the one of the material... and use all this data to get
                //the 4 vertices of the rect that encloses this character. Store them in vertices
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                vertices = textInfo.meshInfo[materialIndex].vertices;

                //Compute the baseline mid point for each character. This is the central point of the character.
                //we will use this as the point representing this character for the geometry transformations
                Vector3 charMidBaselinePos =
                    new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);

                //remove the central point from the vertices point. After this operation, every one of the four vertices 
                //will just have as coordinates the offset from the central position. This will come handy when will deal with the rotations
                vertices[vertexIndex + 0] += -charMidBaselinePos;
                vertices[vertexIndex + 1] += -charMidBaselinePos;
                vertices[vertexIndex + 2] += -charMidBaselinePos;
                vertices[vertexIndex + 3] += -charMidBaselinePos;

                //compute the horizontal position of the character relative to the bounds of the box, in a range [0, 1]
                //where 0 is the left border of the text and 1 is the right border
                float zeroToOnePos = (charMidBaselinePos.x - boundsMinX) / (boundsMaxX - boundsMinX);

                //get the transformation matrix, that maps the vertices, seen as offset from the central character point, to their final
                //position that follows the curve
                matrix = TextWarpFunction.ComputeTransformationMatrix(charMidBaselinePos, zeroToOnePos, textInfo, i);

                //apply the transformation, and obtain the final position and orientation of the 4 vertices representing this char
                vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
            }

            //Upload the mesh with the revised information
            TextMeshProUGUI.UpdateVertexData();
        }
    }
}