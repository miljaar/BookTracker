import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { removeAccessToken } from "./tokenStorage";

export function LogoutButton() {
    const queryClient = useQueryClient();
    const naviqate = useNavigate();

    function logout() {
        removeAccessToken();
        queryClient.removeQueries({ queryKey: ["current-member"] });
        naviqate("/login");
    }

    return <button onClick={logout}>Log out</button>;
}