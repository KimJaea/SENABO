package com.senabo.domain.walk.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.domain.walk.dto.request.UpdateWalkRequest;
import com.senabo.domain.walk.dto.response.TodayWalkResponse;
import com.senabo.domain.walk.dto.response.WalkResponse;
import com.senabo.domain.walk.entity.Walk;
import com.senabo.domain.walk.service.WalkService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/walk")
@Tag(name = "Walk", description = "Walk API Document")
public class WalkController {
    private final WalkService walkService;
    private final MemberService memberService;
    private final ReportService reportService;

    @PostMapping("/start")
    @Operation(summary = "산책 시작", description = "산책을 시작한다. 시작 시간(startTime)을 저장한다.")
    public ApiResponse<WalkResponse> createWalk(@AuthenticationPrincipal UserDetails principal) {
        WalkResponse response = walkService.createWalk(principal.getUsername());
        return ApiResponse.success("산책 시작 성공", response);
    }


    @PutMapping("/end")
    @Operation(summary = "산책 종료", description = "산책을 종료한다. 종료 시간(endTime)과 거리(distance)를 저장한다.")
    public ApiResponse<WalkResponse> updateWalk(@AuthenticationPrincipal UserDetails principal, @RequestBody UpdateWalkRequest request) {
        WalkResponse response = walkService.updateWalk(principal.getUsername(), request);
        return ApiResponse.success("산책 종료 성공", response);
    }

    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = WalkResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND")
    }
    )
    @Operation(summary = "산책 전체 조회", description = "산책 내역을 전체 조회한다.")
    public ApiResponse<List<WalkResponse>> getWalk(@AuthenticationPrincipal UserDetails principal) {
        List<Walk> walk = walkService.getWalk(principal.getUsername());
        if (walk.isEmpty()) return ApiResponse.fail("산책 전체 조회 성공", null);
        List<WalkResponse> response = walk.stream()
                .map(WalkResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("산책 전체 조회 성공", response);
    }

    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = WalkResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND")
    }
    )
    @Operation(summary = "산책 주간 조회", description = "산책 내역을 주간 조회한다.")
    public ApiResponse<List<WalkResponse>> getWalkWeek(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional = reportService.findReportWeek(member, week);
        if (reportOptional.isEmpty()) return ApiResponse.fail("산책 " + week + "주차 조회 실패", null);
        List<Walk> walk = walkService.getWalkWeek(reportOptional.get(), member);
        List<WalkResponse> response = walk.stream()
                .map(WalkResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("산책 " + week + "주차 조회 성공", response);
    }

    @GetMapping("/today")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = TodayWalkResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND")
    }
    )
    @Operation(summary = "산책 당일 조회", description = "산책 내역을 당일 조회한다.")
    public ApiResponse<TodayWalkResponse> getTodayWalk(@AuthenticationPrincipal UserDetails principal) {
        List<Walk> walk = walkService.getTodayWalk(principal.getUsername());
        if (walk.isEmpty()) return ApiResponse.fail("산책 당일 조회 실패", null);
        TodayWalkResponse todayTotalList = walkService.getTodayTotal(walk);
        return ApiResponse.success("산책 당일 조회 성공", todayTotalList);
    }

    @GetMapping("/latest")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "최근 산책 내역 조회 성공", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = WalkResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND")
    }
    )
    @Operation(summary = "최근 산책 조회", description = "최근 산책 내역을 조회한다.")
    public ApiResponse<WalkResponse> getWalkLatest(@AuthenticationPrincipal UserDetails principal) {
        WalkResponse response = walkService.getWalkLatest(principal.getUsername());
        return ApiResponse.success("최근 산책 조회 성공", response);
    }

    @DeleteMapping("/remove")
    @Operation(summary = "산책 내역 삭제", description = "산책 내역을 삭제한다.")
    public ApiResponse<Object> removeWalk(@AuthenticationPrincipal UserDetails principal) {
        walkService.removeWalk(principal.getUsername());
        return ApiResponse.success("산책 삭제 성공", true);
    }
}
