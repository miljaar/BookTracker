import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate, useLocation } from "react-router-dom";
import { ApiError } from "../api";
import { login } from "./authApi";
import { setAccessToken } from "./tokenStorage";

type LoginLocationState = {
    registered?: boolean,
    email?: string
};

export function LoginPage() {
    const location = useLocation();
    const locationState = location.state as LoginLocationState | null;
    const [email, setEmail] = useState(locationState?.email ?? "");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const LoginMutation = useMutation({
        mutationFn: login,
        onSuccess: async (response) => {
            setAccessToken(response.accessToken);
            await queryClient.invalidateQueries({ queryKey: ["currernt-member"] });
            navigate("/account", { replace: true });
        }
    });

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        LoginMutation.mutate({ email, password });
    }

    const invalidCredentials = LoginMutation.error instanceof ApiError && LoginMutation.error.status === 401;

    return (
        <main>
            {locationState?.registered && (
                <p>Your account was created. You can now log in.</p>
            )}
            <h1>Log in</h1>

            <form onSubmit={handleSubmit}>
                <label >
                    Email
                    <input
                        type="email"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        autoComplete="email"
                        required
                    />
                </label>

                <label>Password
                    <input
                        type="password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        autoComplete="current-password"
                        required
                    />
                </label>
                <button type="submit" disabled={LoginMutation.isPending}>
                    {LoginMutation.isPending ? "Logging in..." : "Log in"}
                </button>

                {invalidCredentials && <p>Email or password is incorrect.</p>}
                {LoginMutation.isError && !invalidCredentials && (
                    <p>Login failed. Is the API running?</p>
                )}
            </form>
        </main>
    );
}