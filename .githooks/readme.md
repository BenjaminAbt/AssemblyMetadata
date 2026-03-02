# Git Hooks Setup

This directory contains custom Git hooks for the repository to enforce code quality and workflow standards.

## Available Hooks

### pre-push
Prevents direct pushes to the `main` branch to enforce a pull request workflow. This helps maintain code quality by ensuring all changes go through code review.

## How to Activate Git Hooks

Git hooks need to be activated manually as they are not automatically enabled when cloning a repository. Follow these steps:

### Method 1: Configure Git Hooks Path (Recommended)

1. Navigate to your repository root directory
2. Run the following command to configure Git to use the `.githooks` directory:
   ```bash
   git config core.hooksPath .githooks
   ```

### Method 2: Copy Hooks to .git/hooks Directory

1. Navigate to your repository root directory
2. Copy the hook files to the `.git/hooks` directory:
   
   **On Windows (PowerShell):**
   ```powershell
   Copy-Item .githooks\* .git\hooks\ -Force
   ```
   
   **On macOS/Linux:**
   ```bash
   cp .githooks/* .git/hooks/
   chmod +x .git/hooks/*
   ```

### Verification

To verify that the hooks are properly installed, you can:

1. Check the current hooks path configuration:
   ```bash
   git config core.hooksPath
   ```

2. List the hooks in your hooks directory:
   ```bash
   ls -la .git/hooks/
   # or on Windows:
   dir .git\hooks\
   ```

## Testing the pre-push Hook

To test if the pre-push hook is working:

1. Make sure you're on the `main` branch
2. Try to push directly to main:
   ```bash
   git push origin main
   ```
3. You should see an error message preventing the push

## Bypassing Hooks (Not Recommended)

If you absolutely need to bypass a hook (use with caution):
```bash
git push --no-verify origin main
```

## Troubleshooting

### Hook Not Executing
- Ensure the hook files have execute permissions (especially on macOS/Linux)
- Verify the hooks path is correctly configured
- Check that the hook files don't have a file extension

### Permission Issues on Windows
- Make sure Git Bash or your terminal has the necessary permissions
- Consider running your terminal as administrator if needed

## Best Practices

1. Always work on feature branches
2. Create pull requests for code review
3. Only merge to main through approved pull requests
4. Keep the main branch stable and deployable

For more information about Git hooks, visit: https://git-scm.com/book/en/v2/Customizing-Git-Git-Hooks