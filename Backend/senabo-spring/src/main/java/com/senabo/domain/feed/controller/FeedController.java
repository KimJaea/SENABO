package com.senabo.domain.feed.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.feed.dto.response.CheckFeedResponse;
import com.senabo.domain.feed.dto.response.FeedResponse;
import com.senabo.domain.feed.entity.Feed;
import com.senabo.domain.feed.service.FeedService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
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
@RequestMapping("/feed")
@Tag(name = "Feed", description = "Feed API Document")
public class FeedController {

    private final FeedService feedService;
    private final ReportService reportService;
    private final MemberService memberService;

    @PostMapping("/save")
    @Operation(summary = "배식 저장", description = "배식을 저장한다.")
    public ApiResponse<FeedResponse> createFeed(@AuthenticationPrincipal UserDetails principal) {
        FeedResponse response = feedService.createFeed(principal.getUsername());
        return ApiResponse.success("배식 저장 성공", response);
    }

    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = FeedResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "배식 전체 조회", description = "배식 내역을 전체 조회한다.")
    public ApiResponse<List<FeedResponse>> getFeed(@AuthenticationPrincipal UserDetails principal) {
        List<Feed> feed = feedService.getFeed(principal.getUsername());
        if (feed.isEmpty()) return ApiResponse.fail("배식 전체 조회 실패", null);
        List<FeedResponse> response = feed.stream()
                .map(FeedResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("배식 전체 조회 성공", response);
    }

    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = FeedResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "배식 주간 조회", description = "배식 내역을 주간 조회한다.")
    public ApiResponse<List<FeedResponse>> getFeedWeek(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional = reportService.findReportWeek(member, week);
        if (reportOptional.isEmpty()) return ApiResponse.fail("배식 " + week + "주차 조회 실패", null);
        List<Feed> feed = feedService.getFeedWeek(reportOptional.get(), member);
        List<FeedResponse> response = feed.stream()
                .map(FeedResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("배식 " + week + "주차 조회 성공", response);
    }

    @GetMapping("/check")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "가능하면 possibleYn: ture, 불가능하면 possibleYn: false", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = CheckFeedResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "배식 가능 여부 확인", description = "마지막 배식시간이 현재 시각으로 부터 12시간이 경과했는지 확인한다.")
    public ApiResponse<CheckFeedResponse> checkLastFeed(@AuthenticationPrincipal UserDetails principal) {
        CheckFeedResponse response = feedService.checkLastFeed(principal.getUsername());
        return ApiResponse.success("배식 가능 여부 확인 성공", response);
    }

    @DeleteMapping("/remove")
    @Operation(summary = "배식 내역 삭제", description = "배식 내역을 삭제한다.")
    public ApiResponse<Object> removeFeed(@AuthenticationPrincipal UserDetails principal) {
        feedService.removeFeed(principal.getUsername());
        return ApiResponse.success("배식 삭제 성공", true);
    }

    // 가장 최신 Feed 조회
    @GetMapping("/latest")
    @Operation(summary = "최신 Feed 내역 조회", description = "최신 Feed 내역를 조회한다.")
    public ApiResponse<FeedResponse> getFeedLatest(@AuthenticationPrincipal UserDetails principal) {
        FeedResponse response = feedService.getFeedLatest(principal.getUsername());
        return ApiResponse.success("최신 Feed 내역 조회 성공", response);
    }


    // 배변 Clean PUT
    @PutMapping("/clean")
    @Operation(summary = "배변 청소 완료", description = "배변 청소 여부(cleanYn)를 true로 수정한다.")
    public ApiResponse<FeedResponse> updatePoop(@AuthenticationPrincipal UserDetails principal) {
        FeedResponse response = feedService.updatePoop(principal.getUsername());
        return ApiResponse.success("배변 청소 성공", response);
    }

}
