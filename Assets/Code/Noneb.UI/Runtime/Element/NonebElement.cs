using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using Unity.Logging;
using UnityEngine;

namespace Noneb.UI.Element
{
    public sealed class NonebElement : MonoBehaviour
    {
        public async UniTask Init()
        {
            var eventComponents = FindComponents(transform).ToList();
            var initTasks = eventComponents.Select(c => c.OnInit());

            await UniTask.WhenAll(initTasks);
        }

        public bool IsElementActive { get; private set; }

        public NonebViewBehaviour? OwnerView => GetComponentInParent<NonebViewBehaviour>();

        public async UniTask Activate()
        {
            IsElementActive = true;

            var eventComponents = FindComponents(transform).ToList();
            eventComponents.ForEach(c => c.IsActivated = true);

            var activateEvents = eventComponents.Select(h => h.OnActivate());
            await UniTask.WhenAll(activateEvents);
        }

        public async UniTask Deactivate()
        {
            IsElementActive = false;

            var eventComponents = FindComponents(transform).ToList();
            eventComponents.ForEach(c => c.IsActivated = false);

            var deactivateEvents = eventComponents.Select(h => h.OnDeactivate());
            await UniTask.WhenAll(deactivateEvents);
        }

        private IEnumerable<IElementComponent> FindComponents(Transform t)
        {
            foreach (var handler in t.GetComponents<IElementComponent>()) yield return handler;

            foreach (Transform child in t)
            {
                if (child.GetComponent<NonebElement>() != null) continue;

                foreach (var nestedHandlers in FindComponents(child)) yield return nestedHandlers;
            }
        }

        public static async UniTask<(bool isSuccess, NonebElement element)> CreateElementFromPrefab(NonebElement elementPrefab, Transform elementHolder)
        {
            var createdElement = Instantiate(elementPrefab, elementHolder);
            INonebView? ownerView = createdElement.OwnerView;
            if (ownerView == null)
            {
                Log.Error("You really should instantiate me under a NonebView");
                return (false, createdElement);
            }

            if (ownerView.InitState != INonebView.InitializationState.PreInitialize) return (true, createdElement);

            createdElement.transform.SetParent(elementHolder.transform);
            await createdElement.Init();

            if (!ownerView.IsViewActive) return (true, createdElement);

            await elementPrefab.Activate();

            return (true, createdElement);
        }

        public static async UniTask<(bool isSuccess, T? element)> CreateElementFromPrefab<T>(T elementPrefab, Transform elementHolder) where T : IElementComponent
        {
            if (elementPrefab is not MonoBehaviour monoBehaviour)
            {
                Log.Error("I expected all IElementComponent is a MonoBehaviour, I couldn't find a way to express this in code and this might change, but you should at least not do this as if T is not a MonoBehaviour how can I instantiate a prefab");
                return (false, default);
            }

            if (!monoBehaviour.TryGetComponent(out NonebElement nonebElement))
            {
                Log.Error($"You need a NonebElement for this({monoBehaviour.gameObject.name}) to work mate");
                return (false, default);
            }

            return await CreateElementFromPrefab<T>(nonebElement, elementHolder);
        }

        public static async UniTask<(bool isSuccess, T? element)> CreateElementFromPrefab<T>(NonebElement elementPrefab, Transform elementHolder) where T : IElementComponent
        {
            var (isSuccess, element) = await CreateElementFromPrefab(elementPrefab, elementHolder);
            if (!isSuccess) return (false, default);

            var component = element.GetComponent<T>();
            return (component != null, component);
        }
    }
}