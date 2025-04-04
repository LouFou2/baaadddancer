//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Scripts/PlayerControls.inputactions
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

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""DanceControls"",
            ""id"": ""e38dc9af-913d-4df1-a536-849fdbfe3593"",
            ""actions"": [
                {
                    ""name"": ""MoveL"",
                    ""type"": ""Value"",
                    ""id"": ""e7d64aa6-6835-4c27-bd61-6c4fd737f393"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MoveR"",
                    ""type"": ""Value"",
                    ""id"": ""49b7d592-f520-4db6-951b-7d17750da3ab"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SwitchObjectL"",
                    ""type"": ""Button"",
                    ""id"": ""b6d27f35-8cff-4a52-81f2-2108a692175f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchObjectR"",
                    ""type"": ""Button"",
                    ""id"": ""98a588c7-f0b9-43f4-bcd5-f635e0670095"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Record"",
                    ""type"": ""Button"",
                    ""id"": ""e06d41e8-df3e-4e98-a13b-a6576811643c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Play"",
                    ""type"": ""Button"",
                    ""id"": ""b30910d4-5296-430c-84c6-d7ca041d53a1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateViewX_Top"",
                    ""type"": ""Button"",
                    ""id"": ""cbe11457-02e4-4184-b2fc-5943e996ecb8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateViewX_Front"",
                    ""type"": ""Button"",
                    ""id"": ""8ac653bc-8eac-4707-8a1c-1b3d658bb58b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateViewY_Left"",
                    ""type"": ""Button"",
                    ""id"": ""c8eeb4fe-7111-4bfe-8b8a-2268afceb560"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateViewY_Right"",
                    ""type"": ""Button"",
                    ""id"": ""7ee88f04-dc38-4071-b246-939080c42387"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""YesButton"",
                    ""type"": ""Button"",
                    ""id"": ""767e4032-db96-43b1-94cf-e04bdc6e4515"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NoButton"",
                    ""type"": ""Button"",
                    ""id"": ""3bd1569a-9091-48b2-a32f-816b027244a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AButton"",
                    ""type"": ""Button"",
                    ""id"": ""a459ec09-c6a7-4985-a84d-24f1cf89fa8b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BButton"",
                    ""type"": ""Button"",
                    ""id"": ""eef1626f-cef7-4e03-8a1f-19094103fe42"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3f940430-de04-44de-92ec-e2639b9d485d"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8224df10-41b7-4ea9-82fa-acbd076f5c87"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchObjectL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a19c530-f900-4383-81c3-95e5b3da89d1"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c2703a6-fe47-46d2-84f5-baeec28cbb7a"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchObjectR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8cf1fe23-c3ea-45f1-9f44-9b37765e69d2"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Record"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0118f00b-30ff-42f1-bfd0-f97840cdca86"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Play"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d459a0f-3227-4cb2-9b9e-0b6c55aa117f"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateViewX_Top"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1016fdd3-7aa7-4403-9ac0-eae681a25a6b"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateViewX_Front"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e470bf06-70ec-403c-b421-816e23e23199"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateViewY_Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4f32dd03-cc14-4249-a5af-f5490da0c4ad"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateViewY_Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9725efc6-d341-4701-b781-97507f03632b"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""YesButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e22cbb87-e213-4f0f-9c1a-9a42aa0d797b"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NoButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a9a4d71-13a8-4c45-838f-d663ed9306ee"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55c62404-c681-4005-bcd5-46517d8c4a2c"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""GenericInput"",
            ""id"": ""ac9880aa-036c-4479-9cf9-3a3b1065bfdc"",
            ""actions"": [
                {
                    ""name"": ""YButton"",
                    ""type"": ""Button"",
                    ""id"": ""75c92ea4-c077-479b-ac8d-7b42be5217b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""c9c738f9-c267-4cf9-a21f-3ec77396ec63"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""c60ce82d-b76c-4212-b823-35ad53974f70"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AButton"",
                    ""type"": ""Button"",
                    ""id"": ""3a1faf52-d993-4bb1-82d0-5e42cc73e188"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LBumper"",
                    ""type"": ""Button"",
                    ""id"": ""333049e5-9d4e-4afc-98bd-d92cf6e9d946"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RBumper"",
                    ""type"": ""Button"",
                    ""id"": ""9d193968-ca9f-4faa-96e1-510022c9eb66"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""afcbae49-f90a-47c5-a4a2-97c5e41a4e3c"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""YButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6ad294b-814b-4815-9054-c8b1c156515a"",
                    ""path"": ""<XInputController>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""YButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb958335-28b7-4710-a261-e48ac2469e66"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4cda9cce-25ca-4005-8465-766d2458d838"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15d63802-c048-43eb-915d-7d3c798b454b"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77cb2902-925c-404e-9371-56107c09c3c8"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e6f9809-b9d8-4fcd-af03-e23692138648"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""acd5aacf-f6c4-4299-9a08-4561e034712e"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1981a123-66b3-4cf4-a27c-5f570c8a19e6"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LBumper"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f7ab254-e83e-4216-b742-701667af4fe6"",
                    ""path"": ""<XInputController>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LBumper"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40e42e7a-fafa-4a58-8426-96d0e21ee07c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RBumper"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e694968c-356e-4084-afa5-84d24ad34f5f"",
                    ""path"": ""<XInputController>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RBumper"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // DanceControls
        m_DanceControls = asset.FindActionMap("DanceControls", throwIfNotFound: true);
        m_DanceControls_MoveL = m_DanceControls.FindAction("MoveL", throwIfNotFound: true);
        m_DanceControls_MoveR = m_DanceControls.FindAction("MoveR", throwIfNotFound: true);
        m_DanceControls_SwitchObjectL = m_DanceControls.FindAction("SwitchObjectL", throwIfNotFound: true);
        m_DanceControls_SwitchObjectR = m_DanceControls.FindAction("SwitchObjectR", throwIfNotFound: true);
        m_DanceControls_Record = m_DanceControls.FindAction("Record", throwIfNotFound: true);
        m_DanceControls_Play = m_DanceControls.FindAction("Play", throwIfNotFound: true);
        m_DanceControls_RotateViewX_Top = m_DanceControls.FindAction("RotateViewX_Top", throwIfNotFound: true);
        m_DanceControls_RotateViewX_Front = m_DanceControls.FindAction("RotateViewX_Front", throwIfNotFound: true);
        m_DanceControls_RotateViewY_Left = m_DanceControls.FindAction("RotateViewY_Left", throwIfNotFound: true);
        m_DanceControls_RotateViewY_Right = m_DanceControls.FindAction("RotateViewY_Right", throwIfNotFound: true);
        m_DanceControls_YesButton = m_DanceControls.FindAction("YesButton", throwIfNotFound: true);
        m_DanceControls_NoButton = m_DanceControls.FindAction("NoButton", throwIfNotFound: true);
        m_DanceControls_AButton = m_DanceControls.FindAction("AButton", throwIfNotFound: true);
        m_DanceControls_BButton = m_DanceControls.FindAction("BButton", throwIfNotFound: true);
        // GenericInput
        m_GenericInput = asset.FindActionMap("GenericInput", throwIfNotFound: true);
        m_GenericInput_YButton = m_GenericInput.FindAction("YButton", throwIfNotFound: true);
        m_GenericInput_LTrigger = m_GenericInput.FindAction("LTrigger", throwIfNotFound: true);
        m_GenericInput_RTrigger = m_GenericInput.FindAction("RTrigger", throwIfNotFound: true);
        m_GenericInput_AButton = m_GenericInput.FindAction("AButton", throwIfNotFound: true);
        m_GenericInput_LBumper = m_GenericInput.FindAction("LBumper", throwIfNotFound: true);
        m_GenericInput_RBumper = m_GenericInput.FindAction("RBumper", throwIfNotFound: true);
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

    // DanceControls
    private readonly InputActionMap m_DanceControls;
    private List<IDanceControlsActions> m_DanceControlsActionsCallbackInterfaces = new List<IDanceControlsActions>();
    private readonly InputAction m_DanceControls_MoveL;
    private readonly InputAction m_DanceControls_MoveR;
    private readonly InputAction m_DanceControls_SwitchObjectL;
    private readonly InputAction m_DanceControls_SwitchObjectR;
    private readonly InputAction m_DanceControls_Record;
    private readonly InputAction m_DanceControls_Play;
    private readonly InputAction m_DanceControls_RotateViewX_Top;
    private readonly InputAction m_DanceControls_RotateViewX_Front;
    private readonly InputAction m_DanceControls_RotateViewY_Left;
    private readonly InputAction m_DanceControls_RotateViewY_Right;
    private readonly InputAction m_DanceControls_YesButton;
    private readonly InputAction m_DanceControls_NoButton;
    private readonly InputAction m_DanceControls_AButton;
    private readonly InputAction m_DanceControls_BButton;
    public struct DanceControlsActions
    {
        private @PlayerControls m_Wrapper;
        public DanceControlsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveL => m_Wrapper.m_DanceControls_MoveL;
        public InputAction @MoveR => m_Wrapper.m_DanceControls_MoveR;
        public InputAction @SwitchObjectL => m_Wrapper.m_DanceControls_SwitchObjectL;
        public InputAction @SwitchObjectR => m_Wrapper.m_DanceControls_SwitchObjectR;
        public InputAction @Record => m_Wrapper.m_DanceControls_Record;
        public InputAction @Play => m_Wrapper.m_DanceControls_Play;
        public InputAction @RotateViewX_Top => m_Wrapper.m_DanceControls_RotateViewX_Top;
        public InputAction @RotateViewX_Front => m_Wrapper.m_DanceControls_RotateViewX_Front;
        public InputAction @RotateViewY_Left => m_Wrapper.m_DanceControls_RotateViewY_Left;
        public InputAction @RotateViewY_Right => m_Wrapper.m_DanceControls_RotateViewY_Right;
        public InputAction @YesButton => m_Wrapper.m_DanceControls_YesButton;
        public InputAction @NoButton => m_Wrapper.m_DanceControls_NoButton;
        public InputAction @AButton => m_Wrapper.m_DanceControls_AButton;
        public InputAction @BButton => m_Wrapper.m_DanceControls_BButton;
        public InputActionMap Get() { return m_Wrapper.m_DanceControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DanceControlsActions set) { return set.Get(); }
        public void AddCallbacks(IDanceControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_DanceControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DanceControlsActionsCallbackInterfaces.Add(instance);
            @MoveL.started += instance.OnMoveL;
            @MoveL.performed += instance.OnMoveL;
            @MoveL.canceled += instance.OnMoveL;
            @MoveR.started += instance.OnMoveR;
            @MoveR.performed += instance.OnMoveR;
            @MoveR.canceled += instance.OnMoveR;
            @SwitchObjectL.started += instance.OnSwitchObjectL;
            @SwitchObjectL.performed += instance.OnSwitchObjectL;
            @SwitchObjectL.canceled += instance.OnSwitchObjectL;
            @SwitchObjectR.started += instance.OnSwitchObjectR;
            @SwitchObjectR.performed += instance.OnSwitchObjectR;
            @SwitchObjectR.canceled += instance.OnSwitchObjectR;
            @Record.started += instance.OnRecord;
            @Record.performed += instance.OnRecord;
            @Record.canceled += instance.OnRecord;
            @Play.started += instance.OnPlay;
            @Play.performed += instance.OnPlay;
            @Play.canceled += instance.OnPlay;
            @RotateViewX_Top.started += instance.OnRotateViewX_Top;
            @RotateViewX_Top.performed += instance.OnRotateViewX_Top;
            @RotateViewX_Top.canceled += instance.OnRotateViewX_Top;
            @RotateViewX_Front.started += instance.OnRotateViewX_Front;
            @RotateViewX_Front.performed += instance.OnRotateViewX_Front;
            @RotateViewX_Front.canceled += instance.OnRotateViewX_Front;
            @RotateViewY_Left.started += instance.OnRotateViewY_Left;
            @RotateViewY_Left.performed += instance.OnRotateViewY_Left;
            @RotateViewY_Left.canceled += instance.OnRotateViewY_Left;
            @RotateViewY_Right.started += instance.OnRotateViewY_Right;
            @RotateViewY_Right.performed += instance.OnRotateViewY_Right;
            @RotateViewY_Right.canceled += instance.OnRotateViewY_Right;
            @YesButton.started += instance.OnYesButton;
            @YesButton.performed += instance.OnYesButton;
            @YesButton.canceled += instance.OnYesButton;
            @NoButton.started += instance.OnNoButton;
            @NoButton.performed += instance.OnNoButton;
            @NoButton.canceled += instance.OnNoButton;
            @AButton.started += instance.OnAButton;
            @AButton.performed += instance.OnAButton;
            @AButton.canceled += instance.OnAButton;
            @BButton.started += instance.OnBButton;
            @BButton.performed += instance.OnBButton;
            @BButton.canceled += instance.OnBButton;
        }

        private void UnregisterCallbacks(IDanceControlsActions instance)
        {
            @MoveL.started -= instance.OnMoveL;
            @MoveL.performed -= instance.OnMoveL;
            @MoveL.canceled -= instance.OnMoveL;
            @MoveR.started -= instance.OnMoveR;
            @MoveR.performed -= instance.OnMoveR;
            @MoveR.canceled -= instance.OnMoveR;
            @SwitchObjectL.started -= instance.OnSwitchObjectL;
            @SwitchObjectL.performed -= instance.OnSwitchObjectL;
            @SwitchObjectL.canceled -= instance.OnSwitchObjectL;
            @SwitchObjectR.started -= instance.OnSwitchObjectR;
            @SwitchObjectR.performed -= instance.OnSwitchObjectR;
            @SwitchObjectR.canceled -= instance.OnSwitchObjectR;
            @Record.started -= instance.OnRecord;
            @Record.performed -= instance.OnRecord;
            @Record.canceled -= instance.OnRecord;
            @Play.started -= instance.OnPlay;
            @Play.performed -= instance.OnPlay;
            @Play.canceled -= instance.OnPlay;
            @RotateViewX_Top.started -= instance.OnRotateViewX_Top;
            @RotateViewX_Top.performed -= instance.OnRotateViewX_Top;
            @RotateViewX_Top.canceled -= instance.OnRotateViewX_Top;
            @RotateViewX_Front.started -= instance.OnRotateViewX_Front;
            @RotateViewX_Front.performed -= instance.OnRotateViewX_Front;
            @RotateViewX_Front.canceled -= instance.OnRotateViewX_Front;
            @RotateViewY_Left.started -= instance.OnRotateViewY_Left;
            @RotateViewY_Left.performed -= instance.OnRotateViewY_Left;
            @RotateViewY_Left.canceled -= instance.OnRotateViewY_Left;
            @RotateViewY_Right.started -= instance.OnRotateViewY_Right;
            @RotateViewY_Right.performed -= instance.OnRotateViewY_Right;
            @RotateViewY_Right.canceled -= instance.OnRotateViewY_Right;
            @YesButton.started -= instance.OnYesButton;
            @YesButton.performed -= instance.OnYesButton;
            @YesButton.canceled -= instance.OnYesButton;
            @NoButton.started -= instance.OnNoButton;
            @NoButton.performed -= instance.OnNoButton;
            @NoButton.canceled -= instance.OnNoButton;
            @AButton.started -= instance.OnAButton;
            @AButton.performed -= instance.OnAButton;
            @AButton.canceled -= instance.OnAButton;
            @BButton.started -= instance.OnBButton;
            @BButton.performed -= instance.OnBButton;
            @BButton.canceled -= instance.OnBButton;
        }

        public void RemoveCallbacks(IDanceControlsActions instance)
        {
            if (m_Wrapper.m_DanceControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDanceControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_DanceControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DanceControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DanceControlsActions @DanceControls => new DanceControlsActions(this);

    // GenericInput
    private readonly InputActionMap m_GenericInput;
    private List<IGenericInputActions> m_GenericInputActionsCallbackInterfaces = new List<IGenericInputActions>();
    private readonly InputAction m_GenericInput_YButton;
    private readonly InputAction m_GenericInput_LTrigger;
    private readonly InputAction m_GenericInput_RTrigger;
    private readonly InputAction m_GenericInput_AButton;
    private readonly InputAction m_GenericInput_LBumper;
    private readonly InputAction m_GenericInput_RBumper;
    public struct GenericInputActions
    {
        private @PlayerControls m_Wrapper;
        public GenericInputActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @YButton => m_Wrapper.m_GenericInput_YButton;
        public InputAction @LTrigger => m_Wrapper.m_GenericInput_LTrigger;
        public InputAction @RTrigger => m_Wrapper.m_GenericInput_RTrigger;
        public InputAction @AButton => m_Wrapper.m_GenericInput_AButton;
        public InputAction @LBumper => m_Wrapper.m_GenericInput_LBumper;
        public InputAction @RBumper => m_Wrapper.m_GenericInput_RBumper;
        public InputActionMap Get() { return m_Wrapper.m_GenericInput; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GenericInputActions set) { return set.Get(); }
        public void AddCallbacks(IGenericInputActions instance)
        {
            if (instance == null || m_Wrapper.m_GenericInputActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GenericInputActionsCallbackInterfaces.Add(instance);
            @YButton.started += instance.OnYButton;
            @YButton.performed += instance.OnYButton;
            @YButton.canceled += instance.OnYButton;
            @LTrigger.started += instance.OnLTrigger;
            @LTrigger.performed += instance.OnLTrigger;
            @LTrigger.canceled += instance.OnLTrigger;
            @RTrigger.started += instance.OnRTrigger;
            @RTrigger.performed += instance.OnRTrigger;
            @RTrigger.canceled += instance.OnRTrigger;
            @AButton.started += instance.OnAButton;
            @AButton.performed += instance.OnAButton;
            @AButton.canceled += instance.OnAButton;
            @LBumper.started += instance.OnLBumper;
            @LBumper.performed += instance.OnLBumper;
            @LBumper.canceled += instance.OnLBumper;
            @RBumper.started += instance.OnRBumper;
            @RBumper.performed += instance.OnRBumper;
            @RBumper.canceled += instance.OnRBumper;
        }

        private void UnregisterCallbacks(IGenericInputActions instance)
        {
            @YButton.started -= instance.OnYButton;
            @YButton.performed -= instance.OnYButton;
            @YButton.canceled -= instance.OnYButton;
            @LTrigger.started -= instance.OnLTrigger;
            @LTrigger.performed -= instance.OnLTrigger;
            @LTrigger.canceled -= instance.OnLTrigger;
            @RTrigger.started -= instance.OnRTrigger;
            @RTrigger.performed -= instance.OnRTrigger;
            @RTrigger.canceled -= instance.OnRTrigger;
            @AButton.started -= instance.OnAButton;
            @AButton.performed -= instance.OnAButton;
            @AButton.canceled -= instance.OnAButton;
            @LBumper.started -= instance.OnLBumper;
            @LBumper.performed -= instance.OnLBumper;
            @LBumper.canceled -= instance.OnLBumper;
            @RBumper.started -= instance.OnRBumper;
            @RBumper.performed -= instance.OnRBumper;
            @RBumper.canceled -= instance.OnRBumper;
        }

        public void RemoveCallbacks(IGenericInputActions instance)
        {
            if (m_Wrapper.m_GenericInputActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGenericInputActions instance)
        {
            foreach (var item in m_Wrapper.m_GenericInputActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GenericInputActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GenericInputActions @GenericInput => new GenericInputActions(this);
    public interface IDanceControlsActions
    {
        void OnMoveL(InputAction.CallbackContext context);
        void OnMoveR(InputAction.CallbackContext context);
        void OnSwitchObjectL(InputAction.CallbackContext context);
        void OnSwitchObjectR(InputAction.CallbackContext context);
        void OnRecord(InputAction.CallbackContext context);
        void OnPlay(InputAction.CallbackContext context);
        void OnRotateViewX_Top(InputAction.CallbackContext context);
        void OnRotateViewX_Front(InputAction.CallbackContext context);
        void OnRotateViewY_Left(InputAction.CallbackContext context);
        void OnRotateViewY_Right(InputAction.CallbackContext context);
        void OnYesButton(InputAction.CallbackContext context);
        void OnNoButton(InputAction.CallbackContext context);
        void OnAButton(InputAction.CallbackContext context);
        void OnBButton(InputAction.CallbackContext context);
    }
    public interface IGenericInputActions
    {
        void OnYButton(InputAction.CallbackContext context);
        void OnLTrigger(InputAction.CallbackContext context);
        void OnRTrigger(InputAction.CallbackContext context);
        void OnAButton(InputAction.CallbackContext context);
        void OnLBumper(InputAction.CallbackContext context);
        void OnRBumper(InputAction.CallbackContext context);
    }
}
