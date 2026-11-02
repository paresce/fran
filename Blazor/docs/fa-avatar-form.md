[← Back to index](index.md)

# FaAvatarForm & FaAccountMenu

An avatar-triggered profile, settings, and session menu designed for app headers and topbars across all Paresce applications.

In its resting state, `FaAvatarForm` (and its alias `<FaAccountMenu>`) renders a standard round avatar button (`fa-avatar-form-trigger-round`). You can configure whether the user's name appears alongside the avatar in the topbar or whether only the circular avatar button is shown.

When the user selects their avatar:
- **On Desktop**: It opens a contextual modal anchored **relative to the round avatar button** with an upward pointer notch.
- **On Mobile (`<= 768px`)**: It displays docked at the **bottom of the page as a touch-friendly bottom sheet** with a swipe/drag handle and backdrop overlay.
- **Micro-Interaction vs. Full Page**: Compact settings and theme toggles are handled in-place. If an app's settings exceed this micro-interaction boundary, a prominent link directs the user to a dedicated full page (`/settings` or `/account`), adhering to the Paresce Glanceable Focus Standard.

Inside the opened modal/sheet:
- **User Identity**: Avatar photo or initials, display name, and email or role subtitle.
- **Account Navigation**: A direct action/link to navigate to the user's account form (`AccountHref`).
- **Extensive Settings Link**: An action/link to navigate to full settings (`SettingsHref`, e.g. `/settings`).
- **Application Items**: A slot (`ChildContent`) for any host-application-specific information, navigation links, or actions.
- **Theme Selection**: Built-in Light / Dark / Colorblind mode switcher.
- **Login / Logout Action**: A prominent Log in or Log out action button.

## Usage

### Standard round avatar button (default)

```razor
<FaAvatarForm IsAuthenticated="true"
              DisplayName="Jane Doe"
              ImageUrl="https://example.com/avatar.jpg"
              Email="jane@example.com"
              AccountHref="/account"
              SettingsHref="/settings"
              OnLogout="HandleLogout" />
```

Or using the `<FaAccountMenu>` component:

```razor
<FaAccountMenu IsAuthenticated="true"
               DisplayName="Benjamin Paresce"
               Email="ben@paresce.com"
               AccountHref="/account"
               SettingsHref="/settings"
               OnLogout="HandleLogout" />
```

### Topbar showing user name alongside avatar

Set `ShowDisplayName="true"` to display the user's name next to the avatar in the topbar:

```razor
<FaAvatarForm IsAuthenticated="true"
              DisplayName="Jane Doe"
              ShowDisplayName="true"
              AccountHref="/account"
              OnLogout="HandleLogout" />
```

### With custom application items

Provide custom items or info via `ChildContent`:

```razor
<FaAccountMenu IsAuthenticated="true"
               DisplayName="Jane Doe"
               Email="jane@example.com"
               AccountHref="/account"
               SettingsHref="/settings"
               OnLogout="HandleLogout">
    <a class="fa-avatar-form-item" href="/billing">Billing & Plans</a>
    <a class="fa-avatar-form-item" href="/team">Team Settings</a>
</FaAccountMenu>
```

### Unauthenticated state

```razor
<FaAccountMenu IsAuthenticated="false"
               OnLogin="NavigateToLogin" />
```

## Parameters

| Parameter | Type | Default | Notes |
|---|---|---|---|
| `IsAuthenticated` | `bool` | `false` | When true, renders user identity, account action, and Logout button; when false, renders Guest and Login button |
| `DisplayName` | `string?` | `null` | User display name |
| `ImageUrl` | `string?` | `null` | Photo URL for avatar (falls back to user initials when omitted) |
| `Email` | `string?` | `null` | Secondary text (email, role) displayed in the identity panel |
| `Subtitle` | `string?` | `null` | Alternative secondary text if `Email` is not used |
| `RoundAvatar` | `bool` | `true` | When true (and `ShowDisplayName` is false), renders a circular avatar button |
| `ShowDisplayName` | `bool` | `false` | When false, topbar only shows the circular avatar; when true, shows name alongside avatar |
| `ShowBackdrop` | `bool` | `true` | Renders an interactive backdrop overlay; clicking dismisses the panel |
| `SettingsHref` | `string?` | `null` | URL navigating to a dedicated full settings page when settings exceed modal size |
| `SettingsText` | `string` | `"All settings"` | Label for the settings page link |
| `ShowSettingsLink` | `bool` | `true` | Whether to show the settings page link when configured |
| `OnSettingsClick` | `EventCallback` | | Callback invoked when the settings link is clicked |
| `ShowCaret` | `bool?` | `null` | Dropdown caret visibility; defaults to matching `ShowDisplayName` |
| `ShowThemeSwitcher` | `bool` | `true` | Whether to display Light / Dark / Colorblind switcher in the menu |
| `ThemeSectionTitle` | `string` | `"Theme"` | Title text above the theme switcher |
| `ShowAccountLink` | `bool` | `true` | Whether to show the account action when authenticated |
| `AccountHref` | `string?` | `null` | URL navigating to the account form |
| `OnAccountClick` | `EventCallback` | | Callback invoked when the account action is clicked |
| `AccountText` | `string` | `"Account settings"` | Label for the account action |
| `OnLogin` | `EventCallback` | | Callback invoked on Login click |
| `OnLogout` | `EventCallback` | | Callback invoked on Logout click |
| `LoginText` | `string` | `"Log in"` | Label for Login button |
| `LogoutText` | `string` | `"Log out"` | Label for Logout button |
| `ChildContent` | `RenderFragment?` | `null` | Application-specific items rendered inside the opened avatar form |
| `MenuAlign` | `FaAlign` | `FaAlign.End` | `End` (right-aligned) or `Start` (left-aligned) |
| `CssClass` | `string?` | `null` | Optional CSS class on the container |
| `TriggerCssClass` | `string?` | `null` | Optional CSS class on the trigger button |

[← Back to index](index.md)
