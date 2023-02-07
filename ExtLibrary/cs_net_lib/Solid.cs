using System;
using System.Collections;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace cs_net_lib
{
	public class Solid
	{
		public Solid()
		{
		}

		public class AssemblySolid : cs_net_lib.Solid.SolidBase
		{
			public AssemblySolid(Tekla.Structures.Model.Model m, Assembly a) : base(m, a)
			{
			}

			protected override void CalculateBoundingBox()
			{
				if (this.modelObject != null)
				{
					Assembly assembly = this.modelObject as Assembly;
					if (assembly != null)
					{
						this.boundingBox = new AABB();
						this.CountAssemblyBoundingBox(assembly, ref this.boundingBox);
					}
				}
			}

			private void CountAssemblyBoundingBox(Assembly assembly, ref AABB boundingBox)
			{
				if (assembly == null)
				{
					return;
				}
				AABB aABB = null;
				AABB completeBoundingBox = null;
				AABB completeBoundingBox1 = null;
				ArrayList secondaries = assembly.GetSecondaries();
				ArrayList subAssemblies = assembly.GetSubAssemblies();
				Part mainPart = assembly.GetMainPart() as Part;
				if (mainPart != null)
				{
                    mainPart.Select();
                    aABB = new AABB();
					Tekla.Structures.Model.Solid solid = mainPart.GetSolid();
					aABB.MinPoint = solid.MinimumPoint;
					aABB.MaxPoint = solid.MaximumPoint;
				}
				foreach (ModelObject secondary in secondaries)
				{
                    secondary.Select();
                    AABB minimumPoint = new AABB();
					Part part = secondary as Part;
					if (part != null)
					{
						Tekla.Structures.Model.Solid solid1 = part.GetSolid();
						minimumPoint.MinPoint = solid1.MinimumPoint;
						minimumPoint.MaxPoint = solid1.MaximumPoint;
						completeBoundingBox = base.GetCompleteBoundingBox(minimumPoint, completeBoundingBox);
					}
					Detail detail = secondary as Detail;
					if (detail != null)
					{
						AABB maximumPoint = new AABB();
						cs_net_lib.Solid.DetailSolid detailSolid = new cs_net_lib.Solid.DetailSolid(this.model, detail);
						maximumPoint.MinPoint = detailSolid.MinimumPoint;
						maximumPoint.MaxPoint = detailSolid.MaximumPoint;
						completeBoundingBox = base.GetCompleteBoundingBox(maximumPoint, completeBoundingBox);
					}
					Component component = secondary as Component;
					if (component == null)
					{
						continue;
					}
					AABB aABB1 = new AABB();
					cs_net_lib.Solid.ComponentSolid componentSolid = new cs_net_lib.Solid.ComponentSolid(this.model, component);
					aABB1.MinPoint = componentSolid.MinimumPoint;
					aABB1.MaxPoint = componentSolid.MaximumPoint;
					completeBoundingBox = base.GetCompleteBoundingBox(aABB1, completeBoundingBox);
				}
				foreach (ModelObject subAssembly in subAssemblies)
                {
                    Assembly assembly1 = subAssembly as Assembly;
					if (assembly1 == null)
					{
						continue;
                    }
                    assembly1.Select();
                    AABB minimumPoint1 = new AABB();
					cs_net_lib.Solid.AssemblySolid assemblySolid = new cs_net_lib.Solid.AssemblySolid(this.model, assembly1);
					minimumPoint1.MinPoint = assemblySolid.MinimumPoint;
					minimumPoint1.MaxPoint = assemblySolid.MaximumPoint;
					completeBoundingBox1 = base.GetCompleteBoundingBox(minimumPoint1, completeBoundingBox1);
				}
				AABB aABB2 = new AABB();
				aABB2 = base.GetCompleteBoundingBox(completeBoundingBox1, completeBoundingBox);
				boundingBox = base.GetCompleteBoundingBox(aABB2, aABB);
			}
		}

		public class ComponentSolid : cs_net_lib.Solid.SolidBase
		{
			public ComponentSolid(Tekla.Structures.Model.Model m, Component c) : base(m, c)
			{
			}

			protected override void CalculateBoundingBox()
			{
				if (this.modelObject != null)
				{
					Component component = this.modelObject as Component;
					if (component != null)
					{
						this.boundingBox = new AABB();
						this.CountComponentBoundingBox(component, ref this.boundingBox);
					}
				}
			}

			private void CountComponentBoundingBox(Component component, ref AABB boundingBox)
			{
				if (component == null)
					return;

				AABB completeBoundingBox = null;
				AABB aABB = null;
				ModelObjectEnumerator components = component.GetComponents();
				ModelObjectEnumerator children = component.GetChildren();
				components.SelectInstances = false;
				while (components.MoveNext())
				{
					AABB minimumPoint = new AABB();
					Component current = components.Current as Component;
					cs_net_lib.Solid.ComponentSolid componentSolid = new cs_net_lib.Solid.ComponentSolid(this.model, current);
					minimumPoint.MinPoint = componentSolid.MinimumPoint;
					minimumPoint.MaxPoint = componentSolid.MaximumPoint;
					completeBoundingBox = base.GetCompleteBoundingBox(minimumPoint, completeBoundingBox);
				}
				children.SelectInstances = false;
				while (children.MoveNext())
				{
					AABB maximumPoint = new AABB();
					Part part = children.Current as Part;
					if (part != null)
					{
						Tekla.Structures.Model.Solid solid = part.GetSolid();
						maximumPoint.MinPoint = solid.MinimumPoint;
						maximumPoint.MaxPoint = solid.MaximumPoint;
						aABB = base.GetCompleteBoundingBox(maximumPoint, aABB);
					}
					Detail detail = children.Current as Detail;
					if (detail != null)
					{
						AABB aABB1 = new AABB();
						cs_net_lib.Solid.DetailSolid detailSolid = new cs_net_lib.Solid.DetailSolid(this.model, detail);
						aABB1.MinPoint = detailSolid.MinimumPoint;
						aABB1.MaxPoint = detailSolid.MaximumPoint;
						aABB = base.GetCompleteBoundingBox(aABB1, aABB);
					}
					Component current1 = children.Current as Component;
					if (current1 == null)
					{
						continue;
					}
					AABB minimumPoint1 = new AABB();
					cs_net_lib.Solid.ComponentSolid componentSolid1 = new cs_net_lib.Solid.ComponentSolid(this.model, current1);
					minimumPoint1.MinPoint = componentSolid1.MinimumPoint;
					minimumPoint1.MaxPoint = componentSolid1.MaximumPoint;
					aABB = base.GetCompleteBoundingBox(minimumPoint1, aABB);
				}
				boundingBox = base.GetCompleteBoundingBox(completeBoundingBox, aABB);
			}
		}

		public class DetailSolid : cs_net_lib.Solid.SolidBase
		{
			public DetailSolid(Tekla.Structures.Model.Model m, Detail d) : base(m, d)
			{
			}

			protected override void CalculateBoundingBox()
			{
				if (this.modelObject != null)
				{
					Detail detail = this.modelObject as Detail;
					if (detail != null)
					{
						this.boundingBox = new AABB();
						this.CountDetailBoundingBox(detail, ref this.boundingBox);
					}
				}
			}

			private void CountDetailBoundingBox(Detail detail, ref AABB boundingBox)
			{
				if (detail == null)
				{
					return;
				}
				AABB completeBoundingBox = null;
				ModelObjectEnumerator children = detail.GetChildren();
				children.SelectInstances = false;
				while (children.MoveNext())
				{
					AABB aABB = new AABB();
					Part current = children.Current as Part;
					if (current != null)
					{
						Tekla.Structures.Model.Solid solid = current.GetSolid();
						aABB.MinPoint = solid.MinimumPoint;
						aABB.MaxPoint = solid.MaximumPoint;
						completeBoundingBox = base.GetCompleteBoundingBox(aABB, completeBoundingBox);
					}
					Detail current1 = children.Current as Detail;
					if (current1 != null)
					{
						AABB minimumPoint = new AABB();
						cs_net_lib.Solid.DetailSolid detailSolid = new cs_net_lib.Solid.DetailSolid(this.model, current1);
						minimumPoint.MinPoint = detailSolid.MinimumPoint;
						minimumPoint.MaxPoint = detailSolid.MaximumPoint;
						completeBoundingBox = base.GetCompleteBoundingBox(minimumPoint, completeBoundingBox);
					}
					Component component = children.Current as Component;
					if (component == null)
					{
						continue;
					}
					AABB maximumPoint = new AABB();
					cs_net_lib.Solid.ComponentSolid componentSolid = new cs_net_lib.Solid.ComponentSolid(this.model, component);
					maximumPoint.MinPoint = componentSolid.MinimumPoint;
					maximumPoint.MaxPoint = componentSolid.MaximumPoint;
					completeBoundingBox = base.GetCompleteBoundingBox(maximumPoint, completeBoundingBox);
				}
				if (completeBoundingBox != null)
				{
					boundingBox = completeBoundingBox;
				}
			}
		}

		public abstract class SolidBase
		{
			protected Tekla.Structures.Model.Model model;

			protected ModelObject modelObject;

			protected AABB boundingBox;

			public Point MaximumPoint
			{
				get
				{
					if (this.boundingBox == null)
					{
						this.CalculateBoundingBox();
					}
					if (this.boundingBox == null)
					{
						return null;
					}
					return this.boundingBox.MaxPoint;
				}
			}

			public Point MinimumPoint
			{
				get
				{
					if (this.boundingBox == null)
					{
						this.CalculateBoundingBox();
					}
					if (this.boundingBox == null)
					{
						return null;
					}
					return this.boundingBox.MinPoint;
				}
			}

			public SolidBase(Tekla.Structures.Model.Model m, ModelObject o)
			{
				this.model = m;
				this.boundingBox = null;
				this.SetModelObject(o);
			}

			protected abstract void CalculateBoundingBox();

			protected AABB GetCompleteBoundingBox(AABB bound1, AABB bound2)
			{
				if (bound1 != null && bound2 == null)
				{
					return bound1;
				}
				if (bound1 == null && bound2 != null)
				{
					return bound2;
				}
				if (bound1 == null && bound2 == null)
				{
					return null;
				}
				Point point = new Point();
				Point x = new Point();
				if (!Compare.LT(bound1.MinPoint.X, bound2.MinPoint.X))
				{
					point.X = bound2.MinPoint.X;
				}
				else
				{
					point.X = bound1.MinPoint.X;
				}
				if (!Compare.LT(bound1.MinPoint.Y, bound2.MinPoint.Y))
				{
					point.Y = bound2.MinPoint.Y;
				}
				else
				{
					point.Y = bound1.MinPoint.Y;
				}
				if (!Compare.LT(bound1.MinPoint.Z, bound2.MinPoint.Z))
				{
					point.Z = bound2.MinPoint.Z;
				}
				else
				{
					point.Z = bound1.MinPoint.Z;
				}
				if (!Compare.GT(bound1.MaxPoint.X, bound2.MaxPoint.X))
				{
					x.X = bound2.MaxPoint.X;
				}
				else
				{
					x.X = bound1.MaxPoint.X;
				}
				if (!Compare.GT(bound1.MaxPoint.Y, bound2.MaxPoint.Y))
				{
					x.Y = bound2.MaxPoint.Y;
				}
				else
				{
					x.Y = bound1.MaxPoint.Y;
				}
				if (!Compare.GT(bound1.MaxPoint.Z, bound2.MaxPoint.Z))
				{
					x.Z = bound2.MaxPoint.Z;
				}
				else
				{
					x.Z = bound1.MaxPoint.Z;
				}
				AABB aABB = new AABB()
				{
					MinPoint = new Point(point),
					MaxPoint = new Point(x)
				};
				return aABB;
			}

			public void SetModelObject(ModelObject o)
			{
				this.modelObject = o;
				this.boundingBox = null;
			}
		}
	}
}