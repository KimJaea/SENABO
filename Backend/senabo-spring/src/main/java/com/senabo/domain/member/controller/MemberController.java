package com.senabo.domain.member.controller;

import com.google.firebase.database.core.Repo;
import com.google.protobuf.Api;
import com.senabo.common.api.ApiResponse;
import com.senabo.domain.member.dto.request.*;
import com.senabo.domain.member.dto.response.MemberResponse;
import com.senabo.domain.member.dto.response.ReIssueResponse;
import com.senabo.domain.member.dto.response.SignInResponse;
import com.senabo.domain.member.dto.response.SignUpResponse;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.service.ReportService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/member")
@Tag(name = "Member", description = "Member API Document")
public class MemberController {
    private final MemberService memberService;
    private final ReportService reportService;

    @PostMapping("/sign-up")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "사용자 정보를 받아 회원가입을 한다.", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = SignUpResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "회원가입", description = "토큰과 정보를 받아 회원가입을 한다.")
    public ApiResponse<SignUpResponse> signUp(@RequestBody SignUpRequest request) {
        SignUpResponse response = memberService.signUp(request);
        return ApiResponse.success("회원가입 성공", response);
    }

    @PostMapping("/remove")
    @Operation(summary = "회원 탈퇴", description = "회원정보를 전부 삭제한다.")
    public ApiResponse<Object> remove(@RequestBody SignOutRequest request, @AuthenticationPrincipal UserDetails principal) {
        memberService.removeMember(principal.getUsername(), request);
        return ApiResponse.success("회원탈퇴 성공", true);
    }

    // 로그인
    @PostMapping("/sign-in")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "로그인을 한다. 회원가입을 하지 않았다면 isMember: false / 회원가입을 했다면 isMember: true", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = SignInResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "회원 로그인", description = "구글 OAuth로 로그인을 한다.")
    public ApiResponse<SignInResponse> signIn(@RequestBody SignInRequest firebaseAuthRequest) {
        return ApiResponse.success("회원 로그인", memberService.signIn(firebaseAuthRequest));
    }


    // 로그아웃
    @PostMapping("/sign-out")
    @Operation(summary = "로그아웃", description = "로그아웃을 한다.")
    public ApiResponse<Object> signOut(@RequestBody SignOutRequest request, @AuthenticationPrincipal UserDetails principal) {
        memberService.signOut(request, principal.getUsername());
        return ApiResponse.success("로그아웃 성공", null);
    }


    @GetMapping("/get")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "사용자 정보를 가져온다.", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = MemberResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "회원 정보 조회", description = "회원 정보를 조회한다.")
    public ApiResponse<MemberResponse> getInfo(@AuthenticationPrincipal UserDetails principal) {
        MemberResponse response = memberService.getInfo(principal.getUsername());
        return ApiResponse.success("회원정보 조회 성공", response);
    }

    @PutMapping("/update")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "사용자 정보를 수정한다.", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = MemberResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "회원 정보 수정", description = "강아지 이름, 성별, 종, 위도, 경도를 수정한다.")
    public ApiResponse<MemberResponse> updateInfo(@AuthenticationPrincipal UserDetails principal, @RequestBody UpdateInfoRequest request) {
        MemberResponse response = memberService.updateInfo(principal.getUsername(), request);
        return ApiResponse.success("회원정보 수정 성공", response);
    }

    @PutMapping("/locate")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "사용자 위치를 수정한다.", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = MemberResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "회원 위치 수정", description = "위도, 경도를 수정한다.")
    public ApiResponse<MemberResponse> updateLocate(@AuthenticationPrincipal UserDetails principal, @RequestBody UpdateLocateRequest request) {
        MemberResponse response = memberService.updateLocate(principal.getUsername(), request);
        return ApiResponse.success("회원 위치 수정 성공", response);
    }

    @PostMapping("/total-time")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "사용자 접속시간을 수정한다.", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = MemberResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "회원 접속 정보 수정", description = "총 접속 시간을 수정한다.")
    public ApiResponse<MemberResponse> updateTotalTime(@AuthenticationPrincipal UserDetails principal, @RequestBody TotalTimeRequest request){

        MemberResponse response = memberService.updateTotalTime(principal.getUsername(), request);
        reportService.updateTotalTime(principal.getUsername(), request.totalTime());
        return ApiResponse.success("회원 접속 정보 수정 성공", response);
    }

    @PostMapping("/reissue")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "재발급한 토큰을 가져온다.", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = ReIssueResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "토큰 재발급", description = "만료된 토큰을 받아서 재발급한다.")
    public ApiResponse<ReIssueResponse> reissue(@RequestBody ReIssueRequest request) {
        return ApiResponse.success("토큰 재발급 성공", memberService.reissue(request.refreshToken(), SecurityContextHolder.getContext().getAuthentication()));
    }

    @PostMapping("/fcm")
    public ApiResponse<Object> fcmTest(){
        memberService.fcmTest();
        return ApiResponse.success("FCM 발송 성공", true);
    }

    @GetMapping("/throw/token-check")
    public void throwTokenException(){
        log.info("토큰 Exception");
        memberService.tokenCheckExcpetion();
    }

    @PutMapping("/device-token")
    @Operation(summary = "회원 기기 토큰 수정", description = "기기 토큰을 수정한다.")
    public ApiResponse<MemberResponse> updateDeviceToken(@AuthenticationPrincipal UserDetails principal, @RequestBody UpdateDeviceTokenRequest request){
        MemberResponse response = memberService.updateDeviceToken(principal.getUsername(), request);
        return ApiResponse.success("회원 기기 토큰 수정 성공", response);
    }

    @GetMapping("/throw/not-found")
    public void throwNotFoundException(){
        log.info("Not Found Exception");
        memberService.notFound();
    }
}