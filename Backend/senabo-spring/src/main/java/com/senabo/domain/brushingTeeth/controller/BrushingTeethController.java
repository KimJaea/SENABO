package com.senabo.domain.brushingTeeth.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.brushingTeeth.dto.response.BrushingTeethResponse;
import com.senabo.domain.brushingTeeth.dto.response.CheckBrushingTeethResponse;
import com.senabo.domain.brushingTeeth.entity.BrushingTeeth;
import com.senabo.domain.brushingTeeth.service.BrushingTeethService;
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
@RequestMapping("/brushing-teeth")
@Tag(name = "BrushingTeeth", description = "BrushingTeeth API Document")
public class BrushingTeethController {

    private final BrushingTeethService brushingTeethService;
    private final MemberService memberService;
    private final ReportService reportService;

    @PostMapping("/save")
    @Operation(summary = "양치 내역 저장", description = "양치 완료 시각을 저장한다.")
    public ApiResponse<BrushingTeethResponse> createBrushingTeeth(@AuthenticationPrincipal UserDetails principal) {
        BrushingTeethResponse response = brushingTeethService.createBrushingTeeth(principal.getUsername());
        return ApiResponse.success("양치 내역 저장 성공", response);
    }


    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = BrushingTeethResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND")
    }
    )
    @Operation(summary = "양치 내역 전체 조회", description = "양치 내역을 전체 조회한다.")
    public ApiResponse<List<BrushingTeethResponse>> getBrushingTeeth(@AuthenticationPrincipal UserDetails principal) {
        List<BrushingTeeth> brushingTeeth = brushingTeethService.getBrushingTeeth(principal.getUsername());
        if (brushingTeeth.isEmpty()) return ApiResponse.fail("양치 전체 조회 실패", null);
        List<BrushingTeethResponse> response = brushingTeeth.stream()
                .map(BrushingTeethResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("양치 전체 조회 성공", response);
    }


    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = BrushingTeethResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND")
    }
    )
    @Operation(summary = "양치 내역 주간 조회", description = "양치 내역을 주간 조회한다.")
    public ApiResponse<List<BrushingTeethResponse>> getBrushingTeethWeek(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional = reportService.findReportWeek(member, week);
        if (reportOptional.isEmpty()) return ApiResponse.fail("양치 " + week + "주차 조회 실패", null);
        List<BrushingTeeth> brushingTeeth = brushingTeethService.getBrushingTeethWeek(reportOptional.get(), member);
        List<BrushingTeethResponse> response = brushingTeeth.stream()
                .map(BrushingTeethResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("양치 " + week + "주차 조회 성공", response);
    }


    @DeleteMapping("/remove")
    @Operation(summary = "양치 삭제", description = "양치 내역을 삭제한다.")
    public ApiResponse<Object> removeBrushingTeeth(@AuthenticationPrincipal UserDetails principal) {
        brushingTeethService.removeBrushingTeeth(principal.getUsername());
        return ApiResponse.success("양치 삭제 성공", true);
    }

    // 양치 횟수 일주일 3번 제한 체크
    @GetMapping("/check")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "가능하면 possibleYn: true / 불가능하면 possibleYn: false", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = CheckBrushingTeethResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "DATA NOT FOUND", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = Exception.class))})
    }
    )
    @Operation(summary = "양치 가능 여부 확인", description = "일주일에 3번 미만, 하루에 1번 미만인지 확인하고 가능하면 possibleYn: true / 불가능하면 possibleYn: false를 준다.")
    public ApiResponse<CheckBrushingTeethResponse> checkBrushingTeeth(@AuthenticationPrincipal UserDetails principal) {
        Member member = memberService.findByEmail(principal.getUsername());
        Report report = reportService.findLatestData(member);
        CheckBrushingTeethResponse response = brushingTeethService.checkBrushingTeeth(report, member);
        return ApiResponse.success("양치 가능 여부 확인 성공", response);
    }
}
