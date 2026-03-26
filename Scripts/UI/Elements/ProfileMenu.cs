using Godot;
using System;

public partial class ProfileMenu : MenuButton
{
    [Export] SignInWindow window;

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

            //GameManager.Instance.
        }
        else
        {
            ClearPopupItems();
            popup.AddItem("Sign In", 0);
        }
    }

    void ClearPopupItems()
    {
        for (int i = 0; i < popup.ItemCount; i++)
        {
            popup.RemoveItem(i);
        }
    }

    private void OnProfileOptionSelected(long id)
    {
        //sign in
        if (id == 0)
        {
            window.PopupCentered();
            
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
