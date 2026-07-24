import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import type {
    MemberSummary, GetMemberRequest,
    RegisterMemberRequest, RegisterMemberResponse,
    MemberDetails
} from "./types";

export function registerMember(request: RegisterMemberRequest) {
    return apiRequest<RegisterMemberResponse>("/members", {
        method: "POST",
        body: JSON.stringify(request)
    });
}

export function getMembers(request: GetMemberRequest) {
    const parameters = new URLSearchParams({
        page: request.page.toString(),
        pageSize: request.pageSize.toString()
    });

    if (request.search) {
        parameters.set("search", request.search);
    }

    return apiRequest<PagedResult<MemberSummary>>(`/members?${parameters.toString()}`);
}

export function getMember(memberId: number) {
    return apiRequest<MemberDetails>(`/members/${memberId}`);
}