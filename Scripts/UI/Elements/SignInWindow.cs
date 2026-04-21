using Godot;
using System;

public partial class SignInWindow : Window
{
    [Export] ProfileMenu profileMenu;

    [ExportGroup("UI")]
    [Export] LineEdit usernameEdit;
    [Export] LineEdit passwordEdit;
    [Export] Label errorText;
    [Export] Button signInButton;
    [Export] Button closeButton;
    [Export] Button createAccount;

    public override void _Ready()
	{
        signInButton.Pressed += OnSignInPressed;
        closeButton.Pressed += OnClosePressed;
        CloseRequested += OnClosePressed;

        createAccount.Pressed += OnCreateAccount;

        usernameEdit.TextSubmitted += _ => FocusPassword();
        passwordEdit.TextSubmitted += _ => OnSignInPressed();
	}

    void FocusPassword()
    {
        passwordEdit.GrabFocus();
    }

    async void OnSignInPressed()
    {
        errorText.Visible = false;

        bool success = await GameManager.Instance.Login(new()
        {
            Username = usernameEdit.Text,
            Password = passwordEdit.Text,
        });

        if (success)
        {
            profileMenu.SetLoginState(true);
            Hide();
        }
        else
        {
            errorText.Text = "Username or password incorrect";
            errorText.Visible = true;
        }

    }

    async void OnCreateAccount()
    {
        errorText.Visible = false;

        bool success = await GameManager.Instance.Register(new()
        {
            Username = usernameEdit.Text,
            Password = passwordEdit.Text,
        });

        if (success)
        {
            profileMenu.SetLoginState(true);
            Hide();
        }
        else
        {
            errorText.Text = "Failed to create account";
            errorText.Visible = true;
        }
    }


    void OnClosePressed()
    {
        Hide();
    }
}
