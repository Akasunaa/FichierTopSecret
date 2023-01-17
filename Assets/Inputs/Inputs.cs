//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Inputs/Inputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Inputs : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Inputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Inputs"",
    ""maps"": [
        {
            ""name"": ""onFoot"",
            ""id"": ""70bbff8d-885e-4600-991b-0c58076c2dc7"",
            ""actions"": [
                {
                    ""name"": ""VerticalUpMovement"",
                    ""type"": ""Value"",
                    ""id"": ""c6643643-413c-4493-8798-07e21711239f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""VerticalDownMovement"",
                    ""type"": ""Value"",
                    ""id"": ""0157d371-eff4-4ce2-ad9f-6192d9ae28d2"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HorizontalLeftMovement"",
                    ""type"": ""Value"",
                    ""id"": ""b3e37726-4518-4a79-8234-2ed6fb0d55ca"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HorizontalRightMovement"",
                    ""type"": ""Value"",
                    ""id"": ""c473d8e1-b0fc-4385-8805-f6685c381b4f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""216f78a8-e93f-4804-ac01-b809567db2e4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""6c8434e4-6105-4448-adfa-4e4bac4994b4"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalUpMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""5a9c2297-d918-4f43-b64f-7fcbecc15bed"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalUpMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""a9b64067-ff99-40a5-a663-9602331ce5c5"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalUpMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""dbd93316-d214-4dbc-b8f7-650b76f862a8"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalUpMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""3a7295e6-8756-4ea3-9d8f-ad6826045059"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalLeftMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""18c900d3-adc5-4b20-8e2a-601005eaab23"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalLeftMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ded904e1-96c5-4c6d-af5c-0dd2d4c253a1"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalLeftMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""43d49012-5f07-44bb-9e16-1593f10dfc20"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalLeftMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""12a5eb1f-fa38-49ef-894a-3b3b6ca374c9"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalDownMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""11534717-c5e5-445c-8444-02e9cdfdc7d6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalDownMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""94c49990-fdd6-4426-b100-46b08d9fa2b1"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalDownMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""4275d6e9-c3a2-45bf-bd29-88b29d80daca"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalDownMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""2ce9f2e6-598a-4f10-8818-6ec0a97f40df"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalRightMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c35d3bbd-d854-4f2d-8bd5-6803266e545e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalRightMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""7f6211a5-f1fc-4c2d-8173-048f622fc89f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalRightMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""1a237bac-4071-4894-ad1f-b280a6bd2daa"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalRightMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""767d61f2-e83c-48be-9d50-e06fc71671d0"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""onInteraction"",
            ""id"": ""feea5878-e92c-4cc3-bb8b-f8626d3f9449"",
            ""actions"": [
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""adf36440-9ba6-42ff-8bd1-bee189acf000"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""790ab03e-884a-4189-91de-e5c36242c2f6"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // onFoot
        m_onFoot = asset.FindActionMap("onFoot", throwIfNotFound: true);
        m_onFoot_VerticalUpMovement = m_onFoot.FindAction("VerticalUpMovement", throwIfNotFound: true);
        m_onFoot_VerticalDownMovement = m_onFoot.FindAction("VerticalDownMovement", throwIfNotFound: true);
        m_onFoot_HorizontalLeftMovement = m_onFoot.FindAction("HorizontalLeftMovement", throwIfNotFound: true);
        m_onFoot_HorizontalRightMovement = m_onFoot.FindAction("HorizontalRightMovement", throwIfNotFound: true);
        m_onFoot_Interaction = m_onFoot.FindAction("Interaction", throwIfNotFound: true);
        // onInteraction
        m_onInteraction = asset.FindActionMap("onInteraction", throwIfNotFound: true);
        m_onInteraction_Interaction = m_onInteraction.FindAction("Interaction", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // onFoot
    private readonly InputActionMap m_onFoot;
    private IOnFootActions m_OnFootActionsCallbackInterface;
    private readonly InputAction m_onFoot_VerticalUpMovement;
    private readonly InputAction m_onFoot_VerticalDownMovement;
    private readonly InputAction m_onFoot_HorizontalLeftMovement;
    private readonly InputAction m_onFoot_HorizontalRightMovement;
    private readonly InputAction m_onFoot_Interaction;
    public struct OnFootActions
    {
        private @Inputs m_Wrapper;
        public OnFootActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @VerticalUpMovement => m_Wrapper.m_onFoot_VerticalUpMovement;
        public InputAction @VerticalDownMovement => m_Wrapper.m_onFoot_VerticalDownMovement;
        public InputAction @HorizontalLeftMovement => m_Wrapper.m_onFoot_HorizontalLeftMovement;
        public InputAction @HorizontalRightMovement => m_Wrapper.m_onFoot_HorizontalRightMovement;
        public InputAction @Interaction => m_Wrapper.m_onFoot_Interaction;
        public InputActionMap Get() { return m_Wrapper.m_onFoot; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OnFootActions set) { return set.Get(); }
        public void SetCallbacks(IOnFootActions instance)
        {
            if (m_Wrapper.m_OnFootActionsCallbackInterface != null)
            {
                @VerticalUpMovement.started -= m_Wrapper.m_OnFootActionsCallbackInterface.OnVerticalUpMovement;
                @VerticalUpMovement.performed -= m_Wrapper.m_OnFootActionsCallbackInterface.OnVerticalUpMovement;
                @VerticalUpMovement.canceled -= m_Wrapper.m_OnFootActionsCallbackInterface.OnVerticalUpMovement;
                @VerticalDownMovement.started -= m_Wrapper.m_OnFootActionsCallbackInterface.OnVerticalDownMovement;
                @VerticalDownMovement.performed -= m_Wrapper.m_OnFootActionsCallbackInterface.OnVerticalDownMovement;
                @VerticalDownMovement.canceled -= m_Wrapper.m_OnFootActionsCallbackInterface.OnVerticalDownMovement;
                @HorizontalLeftMovement.started -= m_Wrapper.m_OnFootActionsCallbackInterface.OnHorizontalLeftMovement;
                @HorizontalLeftMovement.performed -= m_Wrapper.m_OnFootActionsCallbackInterface.OnHorizontalLeftMovement;
                @HorizontalLeftMovement.canceled -= m_Wrapper.m_OnFootActionsCallbackInterface.OnHorizontalLeftMovement;
                @HorizontalRightMovement.started -= m_Wrapper.m_OnFootActionsCallbackInterface.OnHorizontalRightMovement;
                @HorizontalRightMovement.performed -= m_Wrapper.m_OnFootActionsCallbackInterface.OnHorizontalRightMovement;
                @HorizontalRightMovement.canceled -= m_Wrapper.m_OnFootActionsCallbackInterface.OnHorizontalRightMovement;
                @Interaction.started -= m_Wrapper.m_OnFootActionsCallbackInterface.OnInteraction;
                @Interaction.performed -= m_Wrapper.m_OnFootActionsCallbackInterface.OnInteraction;
                @Interaction.canceled -= m_Wrapper.m_OnFootActionsCallbackInterface.OnInteraction;
            }
            m_Wrapper.m_OnFootActionsCallbackInterface = instance;
            if (instance != null)
            {
                @VerticalUpMovement.started += instance.OnVerticalUpMovement;
                @VerticalUpMovement.performed += instance.OnVerticalUpMovement;
                @VerticalUpMovement.canceled += instance.OnVerticalUpMovement;
                @VerticalDownMovement.started += instance.OnVerticalDownMovement;
                @VerticalDownMovement.performed += instance.OnVerticalDownMovement;
                @VerticalDownMovement.canceled += instance.OnVerticalDownMovement;
                @HorizontalLeftMovement.started += instance.OnHorizontalLeftMovement;
                @HorizontalLeftMovement.performed += instance.OnHorizontalLeftMovement;
                @HorizontalLeftMovement.canceled += instance.OnHorizontalLeftMovement;
                @HorizontalRightMovement.started += instance.OnHorizontalRightMovement;
                @HorizontalRightMovement.performed += instance.OnHorizontalRightMovement;
                @HorizontalRightMovement.canceled += instance.OnHorizontalRightMovement;
                @Interaction.started += instance.OnInteraction;
                @Interaction.performed += instance.OnInteraction;
                @Interaction.canceled += instance.OnInteraction;
            }
        }
    }
    public OnFootActions @onFoot => new OnFootActions(this);

    // onInteraction
    private readonly InputActionMap m_onInteraction;
    private IOnInteractionActions m_OnInteractionActionsCallbackInterface;
    private readonly InputAction m_onInteraction_Interaction;
    public struct OnInteractionActions
    {
        private @Inputs m_Wrapper;
        public OnInteractionActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Interaction => m_Wrapper.m_onInteraction_Interaction;
        public InputActionMap Get() { return m_Wrapper.m_onInteraction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OnInteractionActions set) { return set.Get(); }
        public void SetCallbacks(IOnInteractionActions instance)
        {
            if (m_Wrapper.m_OnInteractionActionsCallbackInterface != null)
            {
                @Interaction.started -= m_Wrapper.m_OnInteractionActionsCallbackInterface.OnInteraction;
                @Interaction.performed -= m_Wrapper.m_OnInteractionActionsCallbackInterface.OnInteraction;
                @Interaction.canceled -= m_Wrapper.m_OnInteractionActionsCallbackInterface.OnInteraction;
            }
            m_Wrapper.m_OnInteractionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Interaction.started += instance.OnInteraction;
                @Interaction.performed += instance.OnInteraction;
                @Interaction.canceled += instance.OnInteraction;
            }
        }
    }
    public OnInteractionActions @onInteraction => new OnInteractionActions(this);
    public interface IOnFootActions
    {
        void OnVerticalUpMovement(InputAction.CallbackContext context);
        void OnVerticalDownMovement(InputAction.CallbackContext context);
        void OnHorizontalLeftMovement(InputAction.CallbackContext context);
        void OnHorizontalRightMovement(InputAction.CallbackContext context);
        void OnInteraction(InputAction.CallbackContext context);
    }
    public interface IOnInteractionActions
    {
        void OnInteraction(InputAction.CallbackContext context);
    }
}
