using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;


namespace Henge3D
{
    /// <summary>
    /// Compiled polyhedron.
    /// </summary>
    public class CompiledPolyhedron : CompiledPart
    {
        private Vector3[] _body;
        private int[][] _faces;
        private int[][] _neighbors;
        private int[][] _edges;
        private Vector3[] _edgeVectors;
        private Vector3[] _faceNormals;

        /// <summary>
        /// Gets the faces.
        /// </summary>
        /// <value>The faces.</value>
        public int[][] Faces { get { return _faces; } }

        /// <summary>
        /// Gets the neighbors.
        /// </summary>
        /// <value>The neighbors.</value>
        public int[][] Neighbors { get { return _neighbors; } }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        public int[][] Edges { get { return _edges; } }

        /// <summary>
        /// Gets the edge vectors.
        /// </summary>
        /// <value>The edge vectors.</value>
        public Vector3[] EdgeVectors { get { return _edgeVectors; } }

        /// <summary>
        /// Gets the face normals.
        /// </summary>
        /// <value>The face normals.</value>
        public Vector3[] FaceNormals { get { return _faceNormals; } }

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>The body.</value>
        public Vector3[] Body { get { return _body; } }

        private CompiledPolyhedron ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.CompiledPolyhedron"/> class.
        /// </summary>
        /// <param name="vertices">Vertices.</param>
        /// <param name="faces">Faces.</param>
        public CompiledPolyhedron (Vector3[] vertices, int[][] faces)
        {
            this._body = vertices;
            this._faces = faces;

            Vector3 v1, v2, v3;

            _neighbors = new int[vertices.Length][];
            _faceNormals = new Vector3[faces.Length];
            var v2v = new List<int>[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                v2v [i] = new List<int> ();
            var e = new List<int[]> ();
            var ev = new List<Vector3> ();
            for (int i = 0; i < faces.Length; i++)
            {
                int c = faces [i].Length;
                v1 = vertices [faces [i] [0]];
                v2 = vertices [faces [i] [1]];
                v3 = vertices [faces [i] [2]];
                Vector3.Subtract (ref v2, ref v1, out v2);
                Vector3.Subtract (ref v3, ref v1, out v3);
                Vector3.Cross (ref v2, ref v3, out v1);
                v1.Normalize ();
                _faceNormals [i] = v1;

                for (int j = 0; j < c; j++)
                {
                    int n = j == c - 1 ? 0 : j + 1;
                    int p = j == 0 ? c - 1 : j - 1;

                    v1 = _body [faces [i] [j]];
                    v2 = _body [faces [i] [n]];
                    Vector3.Subtract (ref v2, ref v1, out v1);
                    v1.Normalize ();
                    bool isUnique = true;
                    for (int k = 0; k < ev.Count; k++)
                    {
                        v2 = ev [k];
                        float dot;
                        Vector3.Dot (ref v1, ref v2, out dot);
                        if (dot < 0f)
                            dot *= -1f;
                        if (FloatHelper.Abs (dot - 1f) < Constants.Epsilon)
                        {
                            isUnique = false;
                            break;
                        }
                    }
                    if (isUnique)
                        ev.Add (v1);

                    if (!v2v [faces [i] [j]].Contains (faces [i] [n]) &&
                    !v2v [faces [i] [n]].Contains (faces [i] [j]))
                    {
                        e.Add (new int[] { faces [i] [j], faces [i] [n] });
                    }
                    if (j == 0 && !v2v [faces [i] [j]].Contains (faces [i] [p]) &&
                    !v2v [faces [i] [p]].Contains (faces [i] [j]))
                    {
                        e.Add (new int[] { faces [i] [p], faces [i] [j] });
                    }

                    v2v [faces [i] [j]].Add (faces [i] [n]);
                    v2v [faces [i] [j]].Add (faces [i] [p]);
                }
            }
            for (int i = 0; i < v2v.Length; i++)
            {
                _neighbors [i] = v2v [i].Distinct ().ToArray ();
            }
            _edges = e.ToArray ();
            _edgeVectors = ev.ToArray ();
        }

        /// <summary>
        /// Tos the composition part.
        /// </summary>
        /// <returns>The composition part.</returns>
        public override Part ToCompositionPart ()
        {
            return new PolyhedronPart (this);
        }

        /// <summary>
        /// Transform the specified transform.
        /// </summary>
        /// <param name="transform">Transform.</param>
        public override void Transform (ref Matrix transform)
        {
            Vector3.Transform (_body, ref transform, _body);
        }
    }
}
