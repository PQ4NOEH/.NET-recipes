using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public class FreezableSpec
    {
        [Fact]
        public void IsFreezable_should_be_false_for_objects_created_with_ctor()
        {
            var nonFreezablePet = new Pet();
            Assert.False(Freezable.IsFreezable(nonFreezablePet));
        }

        [Fact]
        public void IsFreezable_should_be_true_for_objects_created_with_MakeFreezable()
        {
            var freezablePet = Freezable.MakeFreezable<Pet>();
            Assert.True(Freezable.IsFreezable(freezablePet));
        }

        [Fact]
        public void Freezable_should_work_normally()
        {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 3;
            pet.Deceased = true;
            pet.Name = "Rex";
            pet.Age += pet.Name.Length;
            pet.ToString();
        }

        [Fact]
        public void Frozen_object_should_throw_ObjectFrozenException_when_trying_to_set_a_property()
        {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 3;
  
            Freezable.Freeze(pet);

            Assert.Throws<ObjectFrozenException>(()=> pet.Name = "This should throw");
        }

        [Fact]
        public void Frozen_object_should_not_throw_when_trying_to_read_it()
        {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 3;

            Freezable.Freeze(pet);
            int age = pet.Age;
            string name = pet.Name;
            bool deceased = pet.Deceased;
            string str = pet.ToString();
        }

        [Fact]
        public void Freeze_nonFreezable_object_should_throw_NotFreezableObjectException()
        {
            var rex = new Pet();
            Assert.Throws<NotFreezableObjectException>(() => Freezable.Freeze(rex));
        }

        [Fact]
        public void Freezable_should_not_intercept_property_getters()
        {
            var pet = Freezable.MakeFreezable<Pet>();
            Freezable.Freeze(pet);
            var notUsed = pet.Age; //should not intercept
            var interceptedMethodsCount = GetInterceptedMethodsCountFor(pet);
            Assert.Equal(0, interceptedMethodsCount);
        }
        [Fact]
        public void DynProxyGetTarget_should_return_proxy_itself()
        {
            var pet = Freezable.MakeFreezable<Pet>();
            var hack = pet as IProxyTargetAccessor;
            Assert.NotNull(hack);
            Assert.Same(pet, hack.DynProxyGetTarget());
        }
        [Fact]
        public void Freezable_should_not_hold_any_reference_to_created_objects()
        {
            var pet = Freezable.MakeFreezable<Pet>();
            var petWeakReference = new WeakReference(pet, false);
            pet = null;
            GC.Collect();
            Assert.False(petWeakReference.IsAlive, "Object should have been collected");
        }
        private int GetInterceptedMethodsCountFor(object freezable)
        {
            Assert.True(Freezable.IsFreezable(freezable));

            var hack = freezable as IProxyTargetAccessor;
            Assert.NotNull(hack);
            var loggingInterceptor = (hack.GetInterceptors()
                                        .Where(i => i is FreezableInterceptor)
                                        .FirstOrDefault()) as FreezableInterceptor;
            return loggingInterceptor.InterceptedCallsCount;
        }
    }
}
