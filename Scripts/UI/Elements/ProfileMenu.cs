using Godot;
using System;

public partial class ProfileMenu : MenuButton
{
    [Export] SignInWindow window;

    [ExportGroup("UI")]
    [Export] Label usernameLabel;

    PopupMenu popup;

	public override void _Ready()
	{
        popup = GetPopup();
        popup.IdPressed += OnProfileOptionSelected;

        SetLoginState(GameManager.Instance.IsLoggedIn());
    }

    public void SetLoginState(bool loggedIn)
    {
        if (loggedIn)
        {
            ClearPopupItems();
            popup.AddItem("Sign Out", 1);
            popup.AddItem("View Profile", 2);

            usernameLabel.Text = (string)GameManager.Instance.GetPreference("username");
        }
        else
        {
            ClearPopupItems();
            popup.AddItem("Sign In", 0);
            usernameLabel.Text = "Sign In...";
        }
    }

    void ClearPopupItems()
    {
        for (int i = popup.ItemCount - 1; i >= 0; i--)
        {
            popup.RemoveItem(i);
        }
    }

    private void OnProfileOptionSelected(long id)
    {
        //sign in
        if (id == 0) window.PopupCentered();
        //sign out
        else if (id == 1)
        {
            GameManager.Instance.Logout();
            SetLoginState(false);
        }
    }
}
