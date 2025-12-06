import React, {useEffect, useState} from "react";

const API_BASE = "";

type Category = {
    id: string;
    name: string;
};

type Expense = {
    id: string;
    amount: number;
    date: string; // ISO yyyy-mm-dd
    comment?: string | null;
    categoryId: string;
    categoryName?: string;
};

export default function App(): React.JSX.Element {
    const [categories, setCategories] = useState<Category[]>([]);
    const [expenses, setExpenses] = useState<Expense[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    // Filters & form state
    const [filterCategoryId, setFilterCategoryId] = useState<string | "">("");
    const [filterFrom, setFilterFrom] = useState<string>("");
    const [filterTo, setFilterTo] = useState<string>("");

    const [newExpense, setNewExpense] = useState<Partial<Expense>>({
        amount: undefined,
        date: new Date().toISOString().slice(0, 10),
        comment: "",
        categoryId: undefined,
    });

    const fetchCategories = async () => {
        try {
            const res = await fetch(`${API_BASE}/categories?page=0?size=20`);
            if (!res.ok) throw new Error(`Failed to load categories: ${res.status}`);
            const data = await res.json();
            setCategories(data);
        } catch (e: any) {
            setError(e.message ?? String(e));
        }
    };

    const fetchExpenses = async () => {
        setLoading(true);
        setError(null);
        try {
            const params = new URLSearchParams();
            if (filterCategoryId) params.set("categoryId", filterCategoryId);
            if (filterFrom) params.set("from", filterFrom);
            if (filterTo) params.set("to", filterTo);

            const res = await fetch(`${API_BASE}/expenses?${params.toString()}`);
            if (!res.ok) throw new Error(`Failed to load expenses: ${res.status}`);
            const data = await res.json();
            setExpenses(data);
        } catch (e: any) {
            setError(e.message ?? String(e));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCategories();
        fetchExpenses();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    // Create expense
    const handleCreateExpense = async (e?: React.FormEvent) => {
        e?.preventDefault();
        setError(null);
        try {
            if (!newExpense.amount || !newExpense.date || !newExpense.categoryId) {
                setError("Please provide amount, date and category");
                return;
            }

            const payload = {
                amount: Number(newExpense.amount),
                date: newExpense.date,
                comment: newExpense.comment ?? "",
                categoryId: Number(newExpense.categoryId),
            };

            const res = await fetch(`${API_BASE}/expenses`, {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify(payload),
            });

            if (!res.ok) {
                const text = await res.text();
                throw new Error(`Create failed: ${res.status} ${text}`);
            }

            // reset form
            setNewExpense({
                amount: undefined,
                date: new Date().toISOString().slice(0, 10),
                comment: "",
                categoryId: undefined
            });
            await fetchExpenses();
        } catch (e: any) {
            setError(e.message ?? String(e));
        }
    };

    const handleDeleteExpense = async (id: string) => {
        if (!confirm("Delete this expense?")) return;
        try {
            const res = await fetch(`${API_BASE}/expenses/${id}`, {method: "DELETE"});
            if (!res.ok) throw new Error(`Delete failed: ${res.status}`);
            setExpenses((prev) => prev.filter((x) => x.id !== id));
        } catch (e: any) {
            setError(e.message ?? String(e));
        }
    };

    const handleApplyFilters = async (e?: React.FormEvent) => {
        e?.preventDefault();
        await fetchExpenses();
    };

    const handleClearFilters = () => {
        setFilterCategoryId("");
        setFilterFrom("");
        setFilterTo("");
        fetchExpenses();
    };

    const exportCsv = () => {
        const params = new URLSearchParams();
        if (filterCategoryId) params.set("categoryId", filterCategoryId);
        if (filterFrom) params.set("from", filterFrom);
        if (filterTo) params.set("to", filterTo);
        window.open(`${API_BASE}/export/csv?${params.toString()}`, "_blank");
    };

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-5xl mx-auto">
                <header className="mb-6">
                    <h1 className="text-2xl font-semibold">Expense Tracker — Lite</h1>
                    <p className="text-sm text-gray-600 mt-1">Minimal UI on TypeScript + React. Backend: Minimal API</p>
                </header>

                <main className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    {/* Left: Form */}
                    <section className="md:col-span-1 bg-white p-4 rounded-lg shadow-sm">
                        <h2 className="font-medium mb-3">Add Expense</h2>
                        {error && (
                            <div className="mb-3 text-sm text-red-700 bg-red-50 p-2 rounded">{error}</div>
                        )}
                        <form onSubmit={handleCreateExpense} className="space-y-3">
                            <div>
                                <label className="block text-xs text-gray-600">Amount</label>
                                <input
                                    type="number"
                                    step="0.01"
                                    value={newExpense.amount ?? ""}
                                    onChange={(e) => setNewExpense((s) => ({
                                        ...s,
                                        amount: e.target.value ? Number(e.target.value) : undefined
                                    }))}
                                    className="w-full border rounded px-2 py-1"
                                />
                            </div>

                            <div>
                                <label className="block text-xs text-gray-600">Date</label>
                                <input
                                    type="date"
                                    value={newExpense.date ?? new Date().toISOString().slice(0, 10)}
                                    onChange={(e) => setNewExpense((s) => ({...s, date: e.target.value}))}
                                    className="w-full border rounded px-2 py-1"
                                />
                            </div>

                            <div>
                                <label className="block text-xs text-gray-600">Category</label>
                                <select
                                    value={newExpense.categoryId ?? ""}
                                    onChange={(e) => setNewExpense((s) => ({
                                        ...s,
                                        categoryId: e.target.value ? String(e.target.value) : undefined
                                    }))}
                                    className="w-full border rounded px-2 py-1"
                                >
                                    <option value="">— Select —</option>
                                    {categories.map((c) => (
                                        <option key={c.id} value={c.id}>
                                            {c.name}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <label className="block text-xs text-gray-600">Comment</label>
                                <input
                                    type="text"
                                    value={newExpense.comment ?? ""}
                                    onChange={(e) => setNewExpense((s) => ({...s, comment: e.target.value}))}
                                    className="w-full border rounded px-2 py-1"
                                />
                            </div>

                            <div className="flex gap-2">
                                <button type="submit" className="px-3 py-1 bg-blue-600 text-white rounded">Add</button>
                                <button
                                    type="button"
                                    onClick={() => setNewExpense({
                                        amount: undefined,
                                        date: new Date().toISOString().slice(0, 10),
                                        comment: "",
                                        categoryId: undefined
                                    })}
                                    className="px-3 py-1 border rounded"
                                >
                                    Reset
                                </button>
                            </div>
                        </form>

                        <div className="mt-6">
                            <h3 className="text-sm font-medium mb-2">Categories</h3>
                            <ul className="space-y-1 text-sm">
                                {categories.map((c) => (
                                    <li key={c.id} className="text-gray-700">• {c.name}</li>
                                ))}
                                {categories.length === 0 && <li className="text-gray-400">No categories found</li>}
                            </ul>
                        </div>
                    </section>

                    {/* Right: Expenses + Filters */}
                    <section className="md:col-span-2">
                        <div className="bg-white p-4 rounded-lg shadow-sm mb-4">
                            <h2 className="font-medium mb-3">Filters</h2>
                            <form onSubmit={handleApplyFilters}
                                  className="flex flex-col md:flex-row gap-2 md:items-end">
                                <div className="flex-1">
                                    <label className="block text-xs text-gray-600">Category</label>
                                    <select value={filterCategoryId}
                                            onChange={(e) => setFilterCategoryId(e.target.value)}
                                            className="w-full border rounded px-2 py-1">
                                        <option value="">— All —</option>
                                        {categories.map((c) => (
                                            <option key={c.id} value={String(c.id)}>
                                                {c.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div>
                                    <label className="block text-xs text-gray-600">From</label>
                                    <input type="date" value={filterFrom}
                                           onChange={(e) => setFilterFrom(e.target.value)}
                                           className="border rounded px-2 py-1"/>
                                </div>

                                <div>
                                    <label className="block text-xs text-gray-600">To</label>
                                    <input type="date" value={filterTo} onChange={(e) => setFilterTo(e.target.value)}
                                           className="border rounded px-2 py-1"/>
                                </div>

                                <div className="flex gap-2">
                                    <button type="submit" className="px-3 py-1 bg-green-600 text-white rounded">Apply
                                    </button>
                                    <button type="button" onClick={handleClearFilters}
                                            className="px-3 py-1 border rounded">Clear
                                    </button>
                                    <button type="button" onClick={exportCsv}
                                            className="px-3 py-1 border rounded">Export CSV
                                    </button>
                                </div>
                            </form>
                        </div>

                        <div className="bg-white p-4 rounded-lg shadow-sm">
                            <div className="flex justify-between items-center mb-3">
                                <h2 className="font-medium">Expenses</h2>
                                <div
                                    className="text-sm text-gray-600">{loading ? "Loading..." : `${expenses.length} items`}</div>
                            </div>

                            <table className="w-full text-sm table-auto">
                                <thead className="text-left text-gray-600 text-xs">
                                <tr>
                                    <th className="pb-2">Date</th>
                                    <th className="pb-2">Category</th>
                                    <th className="pb-2">Comment</th>
                                    <th className="pb-2">Amount</th>
                                    <th className="pb-2">Actions</th>
                                </tr>
                                </thead>
                                <tbody>
                                {expenses.length === 0 && (
                                    <tr>
                                        <td colSpan={5} className="py-6 text-center text-gray-400">
                                            No expenses
                                        </td>
                                    </tr>
                                )}

                                {expenses.map((exp) => (
                                    <tr key={exp.id} className="border-t">
                                        <td className="py-2 align-top">{exp.date}</td>
                                        <td className="py-2 align-top">{exp.categoryName ?? categories.find((c) => c.id === exp.categoryId)?.name}</td>
                                        <td className="py-2 align-top">{exp.comment}</td>
                                        <td className="py-2 align-top">{exp.amount.toFixed(2)}</td>
                                        <td className="py-2 align-top">
                                            <div className="flex gap-2">
                                                <button
                                                    onClick={() => navigator.clipboard.writeText(JSON.stringify(exp))}
                                                    className="px-2 py-1 border rounded text-xs">
                                                    Copy
                                                </button>
                                                <button onClick={() => handleDeleteExpense(exp.id)}
                                                        className="px-2 py-1 border rounded text-xs text-red-600">
                                                    Delete
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>
                    </section>
                </main>

                <footer className="mt-6 text-sm text-gray-500">API base: {API_BASE}</footer>
            </div>
        </div>
    );
}
