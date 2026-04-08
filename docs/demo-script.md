## Demo script (quick, recruiter-friendly)

### Accounts to create

Create two users in the UI (Register):

1) **Team Lead** (creator)
   - Email: `teamlead@gmail.com`
   - Password: `Pass123!`

2) **Employee** (assignee)
   - Email: `employee@gmail.com`
   - Password: `Pass123!`

> Note: password rules are relaxed in this app, but keep it consistent for the demo.

---

### Data to create (so screenshots look good)

Login as **Team Lead**.

#### 1) Create lists

Create 2 lists:

- **Home** — "Personal errands and chores"
- **Work** — "Sprint tasks and follow-ups"

Take screenshot: `docs/screenshots/lists.png`

#### 2) Create tasks (inside lists)

Go to **Home → Tasks** and create:

- "Clean kitchen" — Due date: tomorrow — Status: Not Started
- "Pay utilities" — Due date: in 7 days — Status: In Progress
- "Buy groceries" — no due date — Status: Completed

Go to **Work → Tasks** and create:

- "Prepare weekly status update" — Due date: in 2 days — Status: Not Started
- "Review pull request #42" — Due date: today — Status: In Progress

Take screenshot: `docs/screenshots/tasks.png` (shows color by status)

#### 3) Assign a task by email

Open **Work → Tasks**, pick "Prepare weekly status update" → **Edit**

- Set **Assignee (email)** to `employee@gmail.com`
- Save

Take screenshot: `docs/screenshots/assign-by-email.png`

#### 4) Verify Assigned-to-me flow (virtual list)

Logout, login as **Employee**.

Open **My Lists**:
- you should see the virtual list **Assigned to me**
- open **Tasks** from that list OR go to **My Tasks**

Take screenshot: `docs/screenshots/assigned-virtual-list.png`
Take screenshot: `docs/screenshots/assigned-tasks.png`

#### 5) Prove permissions: assignee can only change status

As **Employee**, open assigned task → **Edit**:
- you should see only the status editor
- change status to Completed

Take screenshot: `docs/screenshots/assignee-status-only.png`

Logout, login back as **Team Lead** and verify:
- task is still in the original list
- status changed

Take screenshot: `docs/screenshots/task-details.png`

#### 6) Search demo

Open **Search** and try:
- Title contains: `review`
- Due from/to: pick a range that includes one of your tasks

Take screenshot: `docs/screenshots/search.png`

