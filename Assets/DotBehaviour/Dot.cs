//%GenSrc:1:P+D8lcFaO0uyxGpa3x2lyA
/*
 * This code was generated by InstinctAI.
 *
 * It is safe to edit this file.
 */

using System;
using com.kupio.instinctai;
using UnityEngine.UIElements;

namespace instinctai.usr.behaviours
{
    using UnityEngine;

    public partial class Dot : MonoBehaviour
    {
        [SerializeField] private float size;

        public float Size
        {
            get { return size; }
            set
            {
                size = value;
                transform.localScale = Vector3.one * size / 20f;
            }
        }

        public float GrowUpRate;
        public float SpeciesSpeedFactor;
        public float MateMatureSizeThreshold;
        public float MateSizeDiffThreshold;
        public float MinSize;
        public float MaxSize;
        public float[] ChildrenSizeComposition;

        public CircleCollider2D Collider2D;
        [SerializeField] private SpriteRenderer SpriteRenderer;
        public Rigidbody2D Rigidbody2D;

        public float BasicSpeed => Mathf.Min(100, 50 / Size + 0.01f * Size) * NatureController.Instance.NormalSpeed * SpeciesSpeedFactor;

        public Species.SpeciesTypes M_SpeciesType;
        public Species My_Species;

        public void Init(Species species, float _size, bool randomSize = false)
        {
            if (randomSize)
            {
                Size = Random.Range(MinSize, MaxSize);
            }
            else
            {
                Size = _size;
            }

            My_Species = species;
            M_SpeciesType = species.M_SpeciesType;
            SpriteRenderer.color = NatureController.Instance.ColorSet[(int) M_SpeciesType];
        }

        public bool IsPreyOf(Dot o)
        {
            if (o.M_SpeciesType != M_SpeciesType)
            {
                return Size < NatureController.Instance.EatSizeThreshold * o.Size;
            }
            else
            {
                return false;
            }
        }

        public bool IsPredatorOf(Dot o)
        {
            if (o.M_SpeciesType != M_SpeciesType)
            {
                return Size * NatureController.Instance.EatSizeThreshold > o.Size;
            }
            else
            {
                return false;
            }
        }

        public bool IsMateOf(Dot o)
        {
            if (o.M_SpeciesType == M_SpeciesType && o.Size > MateMatureSizeThreshold && Size > MateMatureSizeThreshold)
            {
                return ((1f - o.MateSizeDiffThreshold) < (Size / o.Size)) && ((Size / o.Size) < (1f + o.MateSizeDiffThreshold));
            }
            else
            {
                return false;
            }
        }

        public bool Destroyed = false;

        public NodeVal GrowUp()
        {
            if (Destroyed) return NodeVal.Success;
            Size = Mathf.Min(MaxSize, GrowUpRate * Size * Time.deltaTime + Size);
            try
            {
                if (Vector2.Distance(transform.position, Vector2.zero) > 540f)
                {
                    NatureController.Instance.DestroyDot(this);
                }
            }
            catch
            {
            }

            return NodeVal.Success;
        }

        public void Valid(bool valid)
        {
            this.valid = valid;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Dot o = other.gameObject.GetComponent<Dot>();
            if (o != null)
            {
                if (o.IsPreyOf(this))
                {
                    Size = (int) Mathf.Sqrt(Size * Size + o.Size * o.Size * NatureController.Instance.NutritionRatio);
                    NatureController.Instance.DestroyDot(o);
                    return;
                }

                if (o.IsMateOf(this))
                {
                    if (My_Species.Dots.Count >= NatureController.Instance.SpeciesCountUpperLimit) return;
                    Dot mother = o.Size > Size ? o : this;
                    Dot father = o.Size > Size ? this : o;
                    float motherLeftSize = 1f;
                    for (int i = 0; i < ChildrenSizeComposition.Length; i++)
                    {
                        motherLeftSize -= ChildrenSizeComposition[i] * ChildrenSizeComposition[i];
                        My_Species.SpawnDot(ChildrenSizeComposition[i] * mother.Size, transform.position, false);
                    }

                    mother.Size = Mathf.Sqrt(motherLeftSize) * mother.Size;
//                    father.Size /= 1.2f;
                    return;
                }
            }
        }

        #region AvoidPredators

        public Vector3 escapingFrom = new Vector3(0, 0);

        public NodeVal FindPredators()
        {
            Dot predator = NatureController.Instance.FindNearestPredator(this);
            if (predator)
            {
                escapingFrom = predator.transform.position;
                return NodeVal.Success;
            }
            else
            {
                return NodeVal.Fail;
            }
        }

        public NodeVal MoveToEscape()
        {
            isWandering = false;
            Rigidbody2D.velocity = Vector3.Normalize(-escapingFrom + transform.position) * BasicSpeed * NatureController.Instance.EscapingSpeedFactor;
            return NodeVal.Success;
        }

        #endregion

        #region ChasePreys  

        public Vector3 preyLocation = new Vector3(0, 0);

        public NodeVal FindPreys()
        {
            Dot prey = NatureController.Instance.FindNearestPrey(this);
            if (prey)
            {
                preyLocation = prey.transform.position;
                return NodeVal.Success;
            }
            else
            {
                return NodeVal.Fail;
            }
        }

        public NodeVal MoveToPrey()
        {
            isWandering = false;
            Rigidbody2D.velocity = Vector3.Normalize(preyLocation - transform.position) * BasicSpeed * NatureController.Instance.ChasingSpeedFactor;
            return NodeVal.Success;
        }

        #endregion

        #region Mate

        public Vector3 mateDestination = new Vector3(0, 0);

        public NodeVal FindMate()
        {
            if (Size < MateMatureSizeThreshold)
            {
                return NodeVal.Fail;
            }

            Dot mateDot = My_Species.FindNearestMate(this);
            if (mateDot != null)
            {
                mateDestination = mateDot.transform.position;
                return NodeVal.Success;
            }
            else
            {
                return NodeVal.Fail;
            }
        }

        public NodeVal MoveToMate()
        {
            isWandering = false;
            Rigidbody2D.velocity = Vector3.Normalize(mateDestination - transform.position) * BasicSpeed * NatureController.Instance.FindingMateSpeedFactor;
            return NodeVal.Success;
        }

        #endregion

        #region  Wandering

        public Vector3 wanderDestination = new Vector3(0, 0);

        public bool isWandering = false;

        public NodeVal Wander()
        {
            if (!isWandering)
            {
                isWandering = true;
                wanderDestination = NatureController.GetRandomPos(Size);
            }
            else
            {
                if (Vector3.Distance(transform.position, wanderDestination) < 5f)
                {
                    isWandering = false;
                }
            }

            return NodeVal.Success;
        }

        public NodeVal MoveToWander()
        {
            Rigidbody2D.velocity = Vector3.Normalize(wanderDestination - transform.position) * BasicSpeed * NatureController.Instance.WanderingSpeedFactor;
            return NodeVal.Success;
        }

        #endregion
    }
}